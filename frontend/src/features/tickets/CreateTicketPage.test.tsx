import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { describe, expect, it } from 'vitest';
import { CreateTicketPage } from './CreateTicketPage';

describe('CreateTicketPage', () => {
  it('shows field validation and enables submission only with valid lengths', async () => {
    const user = userEvent.setup();
    render(<MemoryRouter><CreateTicketPage /></MemoryRouter>);

    const submit = screen.getByRole('button', { name: 'Crear ticket' });
    const title = screen.getByLabelText('Título');
    const description = screen.getByLabelText('Descripción');
    await user.type(title, 'abc');
    await user.type(description, 'breve');

    expect(screen.getByText('Escribe al menos 5 caracteres.')).toBeInTheDocument();
    expect(screen.getByText('Escribe al menos 10 caracteres.')).toBeInTheDocument();
    expect(submit).toBeDisabled();

    await user.type(title, ' válido');
    await user.type(description, ' y suficientemente extensa');
    expect(submit).toBeEnabled();
  });
});
