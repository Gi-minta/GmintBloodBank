import type {
  CreateAppointmentRequest,
  CreateDonationRequest,
  Donation,
  DonationAppointment,
  DonationEvaluation,
  PagedResult,
  RegisterEvaluationRequest,
} from '../types';
import apiClient from './client';

export const donationsApi = {
  list: (params?: { page?: number; pageSize?: number; donorId?: string }) =>
    apiClient.get<PagedResult<Donation>>('/donations', { params }).then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<Donation>(`/donations/${id}`).then((r) => r.data),

  register: (data: CreateDonationRequest) =>
    apiClient.post<Donation>('/donations', data).then((r) => r.data),

  createAppointment: (data: CreateAppointmentRequest) =>
    apiClient.post<DonationAppointment>('/donations/appointments', data).then((r) => r.data),

  registerEvaluation: (data: RegisterEvaluationRequest) =>
    apiClient.post<DonationEvaluation>('/donations/evaluations', data).then((r) => r.data),
};
