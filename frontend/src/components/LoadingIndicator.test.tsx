import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { LoadingIndicator } from './LoadingIndicator';

describe('LoadingIndicator', () => {
  it('announces its loading state without depending on animation', () => {
    render(<LoadingIndicator label="Cargando tickets" />);

    expect(screen.getByRole('status')).toHaveTextContent('Cargando tickets');
  });
});
