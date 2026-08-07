import { describe, expect, it } from 'vitest';
import reducer, { fetchTickets } from './ticketsSlice';
import type { PagedResponse, TicketListItem } from './types';

const result: PagedResponse<TicketListItem> = {
  items: [{
    id: '032b7923-31d4-48ea-8080-b460d898f336',
    title: 'Error de acceso',
    priority: 'High',
    status: 'Open',
    createdAt: '2026-08-03T12:00:00Z',
    updatedAt: '2026-08-03T12:00:00Z',
    createdBy: 'user@example.test',
  }],
  page: 1,
  pageSize: 10,
  totalItems: 1,
  totalPages: 1,
};

describe('ticketsSlice', () => {
  it('tracks loading and stores a successful search', () => {
    const pending = reducer(undefined, fetchTickets.pending('request-id', {
      page: 1, pageSize: 10, sortBy: 'createdAt', sortDirection: 'desc',
    }));
    const fulfilled = reducer(pending, fetchTickets.fulfilled(result, 'request-id', {
      page: 1, pageSize: 10, sortBy: 'createdAt', sortDirection: 'desc',
    }));

    expect(pending.loading).toBe(true);
    expect(fulfilled.loading).toBe(false);
    expect(fulfilled.result.items).toHaveLength(1);
  });
});
