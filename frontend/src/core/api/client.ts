import axios, { AxiosError } from 'axios';
import type { ApiProblem } from '../../features/tickets/types';

function readEnvironment(name: string): string | undefined {
  const value = import.meta.env[name] as unknown;
  return typeof value === 'string' ? value : undefined;
}

export const apiClient = axios.create({
  baseURL: readEnvironment('VITE_API_BASE_URL') ?? 'http://localhost:5080/api',
  timeout: 10_000,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  config.headers['X-User'] = readEnvironment('VITE_CURRENT_USER') ?? 'user@example.test';
  config.headers['X-Correlation-ID'] = crypto.randomUUID();
  return config;
});

export function toApiProblem(error: unknown): ApiProblem {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<Partial<ApiProblem>>;
    if (!axiosError.response) {
      return { title: 'Servicio no disponible', detail: 'No fue posible conectar con el servicio.' };
    }

    const status = axiosError.response.status;
    const defaults: Record<number, Pick<ApiProblem, 'title' | 'detail'>> = {
      400: { title: 'Revisa los datos ingresados', detail: 'La solicitud contiene datos inválidos.' },
      404: { title: 'Recurso no encontrado', detail: 'El recurso solicitado no existe.' },
      409: { title: 'La operación no está permitida', detail: 'El estado actual impide completar la operación.' },
      500: { title: 'Error del servicio', detail: 'Ocurrió un problema interno. Intenta nuevamente.' },
    };
    const fallback = defaults[status] ?? {
      title: 'Solicitud fallida',
      detail: 'No fue posible completar la operación.',
    };

    return {
      title: axiosError.response.data?.title ?? fallback.title,
      detail: axiosError.response.data?.detail ?? fallback.detail,
      status,
      traceId: axiosError.response.data?.traceId,
      errors: axiosError.response.data?.errors,
    };
  }

  return { title: 'Error inesperado', detail: 'Ocurrió un error no esperado.' };
}
