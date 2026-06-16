import type { FeatureFlag, LicenseStatus } from '../types';
import apiClient from './client';

export const licensingApi = {
  getStatus: () =>
    apiClient.get<LicenseStatus>('/licensing/status').then((r) => r.data),

  getFeatures: () =>
    apiClient.get<FeatureFlag[]>('/licensing/features').then((r) => r.data),

  assignLicense: (data: { tenantId: string; licensePlanId: string }) =>
    apiClient.post('/licensing/assign', data).then((r) => r.data),

  toggleFeature: (id: string) =>
    apiClient.post(`/licensing/features/${id}/toggle`).then((r) => r.data),
};
