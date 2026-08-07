import { Link, Navigate, Route, Routes } from 'react-router-dom';
import { CreateTicketPage } from './features/tickets/CreateTicketPage';
import { TicketDetailPage } from './features/tickets/TicketDetailPage';
import { TicketListPage } from './features/tickets/TicketListPage';

export function App() {
  return (
    <>
      <a className="skip-link" href="#main-content">Saltar al contenido</a>
      <header className="app-header">
        <Link className="brand" to="/tickets">
          <span>SD</span>
          SupportDesk
        </Link>
        <p>Gestión de soporte</p>
      </header>
      <Routes>
        <Route path="/tickets" element={<TicketListPage />} />
        <Route path="/tickets/new" element={<CreateTicketPage />} />
        <Route path="/tickets/:id" element={<TicketDetailPage />} />
        <Route path="*" element={<Navigate to="/tickets" replace />} />
      </Routes>
    </>
  );
}
