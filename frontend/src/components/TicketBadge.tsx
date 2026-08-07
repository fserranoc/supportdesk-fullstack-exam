interface Props {
  label: string;
  tone: string;
}

export function TicketBadge({ label, tone }: Props) {
  return <span className={`badge badge--${tone.toLowerCase()}`}>{label}</span>;
}
