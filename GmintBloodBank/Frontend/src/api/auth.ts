import type { LoginRequest, LoginResponse, User } from '../types';
import apiClient from './client';

export const authApi = {
  login: (data: LoginRequest) =>
    apiClient.post<LoginResponse>('/auth/login', data).then((r) => r.data),

  refresh: (refreshToken: string) =>
    apiClient.post<LoginResponse>('/auth/refresh', { refreshToken }).then((r) => r.data),

  me: () =>
    apiClient.get<User>('/auth/me').then((r) => r.data),
};
