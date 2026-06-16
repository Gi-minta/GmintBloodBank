import type { DonationsReport, StockSummary } from '../types';
import apiClient from './client';

export const reportsApi = {
  getStockSummary: () =>
    apiClient.get<StockSummary[]>('/reports/stock-summary').then((r) => r.data),

  getDonationsReport: (params?: { from?: string; to?: string }) =>
    apiClient.get<DonationsReport[]>('/reports/donations', { params }).then((r) => r.data),
};
