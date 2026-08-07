import { useEffect, useMemo, useState } from 'react';
import { Link, useLocation, useSearchParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../app/store';
import { LoadingIndicator } from '../../components/LoadingIndicator';
import { StatusMessage } from '../../components/StatusMessage';
import { TicketBadge } from '../../components/TicketBadge';
import { fetchTickets } from './ticketsSlice';
import type { TicketPriority, TicketStatus } from './types';

const priorities: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];
const statuses: TicketStatus[] = ['Open', 'InProgress', 'Resolved', 'Closed'];

export function TicketListPage() {
  const dispatch = useAppDispatch();
  const location = useLocation();
  const { result, loading, error } = useAppSelector((state) => state.tickets);
  const [params, setParams] = useSearchParams();
  const [query, setQuery] = useState(params.get('q') ?? '');
  const page = Math.max(Number(params.get('page') ?? 1), 1);

  const filters = useMemo(
    () => ({
      status: (params.get('status') || undefined) as TicketStatus | undefined,
      priority: (params.get('priority') || undefined) as TicketPriority | undefined,
      q: query || undefined,
      page,
      pageSize: 10,
      sortBy: 'createdAt',
      sortDirection: 'desc' as const,
    }),
    [page, params, query],
  );

  useEffect(() => {
    const timer = window.setTimeout(() => void dispatch(fetchTickets(filters)), 300);
    return () => window.clearTimeout(timer);
  }, [dispatch, filters]);

  function updateParam(name: string, value: string) {
    const next = new URLSearchParams(params);
    if (value) {
      next.set(name, value);
    } else {
      next.delete(name);
    }
    next.set('page', '1');
    setParams(next);
  }

  function changePage(nextPage: number) {
    const next = new URLSearchParams(params);
    next.set('page', String(nextPage));
    setParams(next);
  }

  return (
    <main className="page" id="main-content" aria-busy={loading}>
      <header className="page-heading">
        <div>
          <p className="eyebrow">Centro de soporte</p>
          <h1>Tickets</h1>
          <p>Consulta y gestiona solicitudes del equipo.</p>
        </div>
        <Link className="button button--primary" to="/tickets/new">Crear ticket</Link>
      </header>

      <section className="filters" aria-label="Filtros de tickets">
        <label>
          Buscar
          <input
            value={query}
            maxLength={200}
            onChange={(event) => {
              setQuery(event.target.value);
              updateParam('q', event.target.value);
            }}
            placeholder="Título o descripción"
          />
        </label>
        <label>
          Estado
          <select value={params.get('status') ?? ''} onChange={(event) => updateParam('status', event.target.value)}>
            <option value="">Todos</option>
            {statuses.map((status) => <option key={status}>{status}</option>)}
          </select>
        </label>
        <label>
          Prioridad
          <select value={params.get('priority') ?? ''} onChange={(event) => updateParam('priority', event.target.value)}>
            <option value="">Todas</option>
            {priorities.map((priority) => <option key={priority}>{priority}</option>)}
          </select>
        </label>
      </section>

      {loading && <LoadingIndicator label="Cargando tickets" />}
      {!loading && error && (
        <StatusMessage
          kind="error"
          title={error.title}
          traceId={error.traceId}
          onRetry={() => void dispatch(fetchTickets(filters))}
        >
          {error.detail}
        </StatusMessage>
      )}
      {!loading && !error && result.items.length === 0 && (
        <StatusMessage title="No hay tickets">
          Prueba cambiando los filtros o crea el primer ticket.
        </StatusMessage>
      )}

      {!loading && !error && result.items.length > 0 && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Título</th>
                <th>Prioridad</th>
                <th>Estado</th>
                <th>Creado</th>
                <th><span className="sr-only">Acciones</span></th>
              </tr>
            </thead>
            <tbody>
              {result.items.map((ticket) => (
                <tr key={ticket.id}>
                  <td data-label="Título">{ticket.title}</td>
                  <td data-label="Prioridad"><TicketBadge label={ticket.priority} tone={ticket.priority} /></td>
                  <td data-label="Estado"><TicketBadge label={ticket.status} tone={ticket.status} /></td>
                  <td data-label="Creado">{new Date(ticket.createdAt).toLocaleString('es-CL')}</td>
                  <td><Link to={`/tickets/${ticket.id}${location.search}`}>Ver detalle</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <nav className="pagination" aria-label="Paginación">
        <button disabled={page <= 1 || loading} onClick={() => changePage(page - 1)}>Anterior</button>
        <span>Página {page} de {Math.max(result.totalPages, 1)}</span>
        <button disabled={page >= result.totalPages || loading} onClick={() => changePage(page + 1)}>Siguiente</button>
      </nav>
    </main>
  );
}
