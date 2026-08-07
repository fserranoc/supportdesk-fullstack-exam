import { apiClient } from '../../core/api/client';
import type {
  CreateTicketInput,
  PagedResponse,
  TicketComment,
  TicketDetail,
  TicketFilters,
  TicketListItem,
  TicketStatus,
} from './types';

export const ticketsApi = {
  async search(filters: TicketFilters): Promise<PagedResponse<TicketListItem>> {
    const { data } = await apiClient.get<PagedResponse<TicketListItem>>('/tickets', { params: filters });
    return data;
  },
  async get(id: string): Promise<TicketDetail> {
    const { data } = await apiClient.get<TicketDetail>(`/tickets/${id}`);
    return data;
  },
  async create(input: CreateTicketInput): Promise<TicketDetail> {
    const { data } = await apiClient.post<TicketDetail>('/tickets', input);
    return data;
  },
  async changeStatus(id: string, status: TicketStatus): Promise<TicketDetail> {
    const { data } = await apiClient.patch<TicketDetail>(`/tickets/${id}/status`, { status });
    return data;
  },
  async comments(id: string): Promise<TicketComment[]> {
    const { data } = await apiClient.get<TicketComment[]>(`/tickets/${id}/comments`);
    return data;
  },
  async addComment(id: string, text: string): Promise<TicketComment> {
    const { data } = await apiClient.post<TicketComment>(`/tickets/${id}/comments`, { text });
    return data;
  },
};
