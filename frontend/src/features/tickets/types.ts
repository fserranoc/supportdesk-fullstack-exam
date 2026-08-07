export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TicketStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed';

export interface TicketListItem {
  id: string;
  title: string;
  priority: TicketPriority;
  status: TicketStatus;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
}

export interface TicketDetail extends TicketListItem {
  description: string;
}

export interface TicketComment {
  id: string;
  ticketId: string;
  text: string;
  createdAt: string;
  createdBy: string;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface TicketFilters {
  status?: TicketStatus;
  priority?: TicketPriority;
  q?: string;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

export interface CreateTicketInput {
  title: string;
  description: string;
  priority: TicketPriority;
}

export interface ApiProblem {
  title: string;
  detail: string;
  status?: number;
  traceId?: string;
  errors?: Record<string, string[]>;
}
