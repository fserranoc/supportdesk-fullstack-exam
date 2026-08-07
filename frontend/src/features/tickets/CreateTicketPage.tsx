import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { StatusMessage } from '../../components/StatusMessage';
import { toApiProblem } from '../../core/api/client';
import { ticketsApi } from './ticketsApi';
import type { ApiProblem, TicketPriority } from './types';

export function CreateTicketPage() {
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TicketPriority>('Medium');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<ApiProblem | null>(null);
  const titleError = title.trim().length > 0 && title.trim().length < 5
    ? 'Escribe al menos 5 caracteres.'
    : '';
  const descriptionError = description.trim().length > 0 && description.trim().length < 10
    ? 'Escribe al menos 10 caracteres.'
    : '';

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (title.trim().length < 5 || description.trim().length < 10) return;

    setSubmitting(true);
    setError(null);
    try {
      const created = await ticketsApi.create({ title, description, priority });
      navigate(`/tickets/${created.id}`, { state: { message: 'Ticket creado correctamente.' } });
    } catch (caught) {
      setError(toApiProblem(caught));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="page page--narrow" id="main-content" aria-busy={submitting}>
      <Link to="/tickets">← Volver al listado</Link>
      <div className="page-heading">
        <div>
          <p className="eyebrow">Nueva solicitud</p>
          <h1>Crear ticket</h1>
        </div>
      </div>
      {error && (
        <StatusMessage kind="error" title={error.title} traceId={error.traceId}>
          {error.detail}
        </StatusMessage>
      )}
      <form className="card form" onSubmit={(event) => void submit(event)} noValidate>
        <label>
          Título
          <input
            required
            minLength={5}
            maxLength={120}
            value={title}
            onChange={(event) => setTitle(event.target.value)}
            aria-describedby="title-help"
          />
          {titleError && <span id="title-help" className="field-error">{titleError}</span>}
        </label>
        <label>
          Descripción
          <textarea
            required
            minLength={10}
            maxLength={2000}
            rows={7}
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            aria-describedby="description-help"
          />
          {descriptionError && <span id="description-help" className="field-error">{descriptionError}</span>}
        </label>
        <label>
          Prioridad
          <select value={priority} onChange={(event) => setPriority(event.target.value as TicketPriority)}>
            <option>Low</option>
            <option>Medium</option>
            <option>High</option>
            <option>Critical</option>
          </select>
        </label>
        <div className="form-actions">
          <Link className="button" to="/tickets">Cancelar</Link>
          <button
            className="button button--primary"
            disabled={submitting || title.trim().length < 5 || description.trim().length < 10}
          >
            {submitting ? 'Creando…' : 'Crear ticket'}
          </button>
        </div>
      </form>
    </main>
  );
}
