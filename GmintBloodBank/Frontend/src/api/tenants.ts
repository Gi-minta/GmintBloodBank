import type { CreateTenantRequest, Tenant } from '../types';
import apiClient from './client';

export const tenantsApi = {
  list: () =>
    apiClient.get<Tenant[]>('/tenants').then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<Tenant>(`/tenants/${id}`).then((r) => r.data),

  create: (data: CreateTenantRequest) =>
    apiClient.post<Tenant>('/tenants', data).then((r) => r.data),

  update: (id: string, data: CreateTenantRequest) =>
    apiClient.put<Tenant>(`/tenants/${id}`, data).then((r) => r.data),
};
