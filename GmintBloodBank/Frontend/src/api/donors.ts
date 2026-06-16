import type { CreateDonorRequest, Donor, PagedResult, UpdateDonorRequest } from '../types';
import apiClient from './client';

export const donorsApi = {
  list: (params?: { page?: number; pageSize?: number; search?: string }) =>
    apiClient.get<PagedResult<Donor>>('/donors', { params }).then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<Donor>(`/donors/${id}`).then((r) => r.data),

  create: (data: CreateDonorRequest) =>
    apiClient.post<Donor>('/donors', data).then((r) => r.data),

  update: (id: string, data: UpdateDonorRequest) =>
    apiClient.put<Donor>(`/donors/${id}`, data).then((r) => r.data),
};
