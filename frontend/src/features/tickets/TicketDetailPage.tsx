import { useCallback, useEffect, useState, type FormEvent } from 'react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { LoadingIndicator } from '../../components/LoadingIndicator';
import { StatusMessage } from '../../components/StatusMessage';
import { TicketBadge } from '../../components/TicketBadge';
import { toApiProblem } from '../../core/api/client';
import { ticketsApi } from './ticketsApi';
import type { ApiProblem, TicketComment, TicketDetail, TicketStatus } from './types';

const nextStatus: Partial<Record<TicketStatus, TicketStatus>> = {
  Open: 'InProgress',
  InProgress: 'Resolved',
  Resolved: 'Closed',
};

export function TicketDetailPage() {
  const { id = '' } = useParams();
  const location = useLocation();
  const [ticket, setTicket] = useState<TicketDetail | null>(null);
  const [comments, setComments] = useState<TicketComment[]>([]);
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(true);
  const [working, setWorking] = useState(false);
  const [error, setError] = useState<ApiProblem | null>(null);
  const successMessage = (location.state as { message?: string } | null)?.message;

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [ticketResult, commentsResult] = await Promise.all([
        ticketsApi.get(id),
        ticketsApi.comments(id),
      ]);
      setTicket(ticketResult);
      setComments(commentsResult);
    } catch (caught) {
      setError(toApiProblem(caught));
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), 0);
    return () => window.clearTimeout(timer);
  }, [load]);

  async function advanceStatus() {
    if (!ticket) return;
    const targetStatus = nextStatus[ticket.status];
    if (!targetStatus) return;

    setWorking(true);
    setError(null);
    try {
      setTicket(await ticketsApi.changeStatus(ticket.id, targetStatus));
    } catch (caught) {
      setError(toApiProblem(caught));
    } finally {
      setWorking(false);
    }
  }

  async function addComment(event: FormEvent) {
    event.preventDefault();
    if (text.trim().length < 2) return;

    setWorking(true);
    setError(null);
    try {
      const created = await ticketsApi.addComment(id, text);
      setComments((items) => [...items, created]);
      setText('');
    } catch (caught) {
      setError(toApiProblem(caught));
    } finally {
      setWorking(false);
    }
  }

  if (loading) {
    return (
      <main className="page" id="main-content" aria-busy="true">
        <LoadingIndicator label="Cargando detalle del ticket" />
      </main>
    );
  }

  if (error?.status === 404) {
    return (
      <main className="page" id="main-content">
        <StatusMessage kind="error" title="Ticket no encontrado" traceId={error.traceId}>
          El recurso solicitado no existe.
        </StatusMessage>
        <Link to="/tickets">Volver al listado</Link>
      </main>
    );
  }

  if (!ticket) {
    return (
      <main className="page" id="main-content">
        <StatusMessage
          kind="error"
          title={error?.title ?? 'No fue posible cargar'}
          traceId={error?.traceId}
          onRetry={() => void load()}
        >
          {error?.detail}
        </StatusMessage>
      </main>
    );
  }

  const following = nextStatus[ticket.status];

  return (
    <main className="page" id="main-content" aria-busy={working}>
      <Link to={`/tickets${location.search}`}>← Volver al listado</Link>
      {successMessage && <StatusMessage kind="success" title={successMessage} />}
      {error && (
        <StatusMessage kind="error" title={error.title} traceId={error.traceId}>
          {error.detail}
        </StatusMessage>
      )}

      <header className="page-heading">
        <div>
          <p className="eyebrow">Ticket</p>
          <h1>{ticket.title}</h1>
          <div className="badge-row">
            <TicketBadge label={ticket.priority} tone={ticket.priority} />
            <TicketBadge label={ticket.status} tone={ticket.status} />
          </div>
        </div>
        {following && (
          <button className="button button--primary" disabled={working} onClick={() => void advanceStatus()}>
            Avanzar a {following}
          </button>
        )}
      </header>

      <section className="card ticket-detail">
        <h2>Descripción</h2>
        <p>{ticket.description}</p>
        <dl>
          <div>
            <dt>Creado por</dt>
            <dd>{ticket.createdBy}</dd>
          </div>
          <div>
            <dt>Fecha de creación</dt>
            <dd>{new Date(ticket.createdAt).toLocaleString('es-CL')}</dd>
          </div>
          <div>
            <dt>Última actualización</dt>
            <dd>{new Date(ticket.updatedAt).toLocaleString('es-CL')}</dd>
          </div>
        </dl>
      </section>

      <section className="comments">
        <h2>Comentarios</h2>
        {comments.length === 0 ? (
          <StatusMessage title="Sin comentarios">Agrega contexto para ayudar al equipo.</StatusMessage>
        ) : (
          <ol>
            {comments.map((comment) => (
              <li className="card" key={comment.id}>
                <p>{comment.text}</p>
                <small>{comment.createdBy} · {new Date(comment.createdAt).toLocaleString('es-CL')}</small>
              </li>
            ))}
          </ol>
        )}

        {ticket.status === 'Closed' ? (
          <StatusMessage title="Ticket cerrado">Ya no se admiten comentarios.</StatusMessage>
        ) : (
          <form className="card form" onSubmit={(event) => void addComment(event)}>
            <label>
              Nuevo comentario
              <textarea
                value={text}
                minLength={2}
                maxLength={1000}
                rows={4}
                onChange={(event) => setText(event.target.value)}
              />
            </label>
            <button className="button button--primary" disabled={working || text.trim().length < 2}>
              {working ? 'Guardando…' : 'Agregar comentario'}
            </button>
          </form>
        )}
      </section>
    </main>
  );
}
