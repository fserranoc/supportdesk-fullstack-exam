interface Props {
  kind?: 'info' | 'error' | 'success';
  title: string;
  children?: React.ReactNode;
  traceId?: string;
  onRetry?: () => void;
}

export function StatusMessage({ kind = 'info', title, children, traceId, onRetry }: Props) {
  return (
    <section className={`status-message status-message--${kind}`} role={kind === 'error' ? 'alert' : 'status'}>
      <strong>{title}</strong>
      {children && <p>{children}</p>}
      {traceId && <small>Identificador de seguimiento: {traceId}</small>}
      {onRetry && <button onClick={onRetry}>Reintentar</button>}
    </section>
  );
}
