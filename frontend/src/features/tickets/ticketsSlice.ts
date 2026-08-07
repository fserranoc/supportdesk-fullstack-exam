import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { toApiProblem } from '../../core/api/client';
import { ticketsApi } from './ticketsApi';
import type { ApiProblem, PagedResponse, TicketFilters, TicketListItem } from './types';

interface TicketsState {
  result: PagedResponse<TicketListItem>;
  loading: boolean;
  error: ApiProblem | null;
}

const initialState: TicketsState = {
  result: { items: [], page: 1, pageSize: 20, totalItems: 0, totalPages: 0 },
  loading: false,
  error: null,
};

export const fetchTickets = createAsyncThunk('tickets/search', async (filters: TicketFilters, { rejectWithValue }) => {
  try {
    return await ticketsApi.search(filters);
  } catch (error) {
    return rejectWithValue(toApiProblem(error));
  }
});

const ticketsSlice = createSlice({
  name: 'tickets',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchTickets.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTickets.fulfilled, (state, action) => {
        state.loading = false;
        state.result = action.payload;
      })
      .addCase(fetchTickets.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as ApiProblem;
      });
  },
});

export default ticketsSlice.reducer;
