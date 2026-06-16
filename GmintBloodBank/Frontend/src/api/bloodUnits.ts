import type { BloodUnit, RegisterBloodUnitRequest, RegisterScreeningRequest } from '../types';
import apiClient from './client';

export const bloodUnitsApi = {
  getAvailable: () =>
    apiClient.get<BloodUnit[]>('/blood-units/available').then((r) => r.data),

  getByCode: (code: string) =>
    apiClient.get<BloodUnit>(`/blood-units/${code}`).then((r) => r.data),

  register: (data: RegisterBloodUnitRequest) =>
    apiClient.post<BloodUnit>('/blood-units', data).then((r) => r.data),

  registerScreening: (id: string, data: RegisterScreeningRequest) =>
    apiClient.post(`/blood-units/${id}/screening`, data).then((r) => r.data),

  release: (id: string) =>
    apiClient.post(`/blood-units/${id}/release`).then((r) => r.data),

  discard: (id: string) =>
    apiClient.post(`/blood-units/${id}/discard`).then((r) => r.data),
};
