import type { ExpiringUnit, InventoryItem, InventoryMovement, RegisterMovementRequest } from '../types';
import apiClient from './client';

export const inventoryApi = {
  getInventory: () =>
    apiClient.get<InventoryItem[]>('/inventory').then((r) => r.data),

  getExpiring: () =>
    apiClient.get<ExpiringUnit[]>('/inventory/expiring').then((r) => r.data),

  registerMovement: (data: RegisterMovementRequest) =>
    apiClient.post<InventoryMovement>('/inventory/movements', data).then((r) => r.data),
};
