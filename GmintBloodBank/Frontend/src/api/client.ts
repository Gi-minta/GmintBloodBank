import axios from 'axios';
import { useToastStore } from '../store/toastStore';

const apiClient = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const { data } = await axios.post('/api/auth/refresh', { refreshToken });
          localStorage.setItem('accessToken', data.accessToken);
          localStorage.setItem('refreshToken', data.refreshToken);
          originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
          return apiClient(originalRequest);
        } catch {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          window.location.href = '/login';
        }
      }
    }

    if (error.response) {
      const status = error.response.status;
      if (status === 403) {
        useToastStore.getState().addToast('No tienes permisos para esta acción.', 'error');
      } else if (status === 404) {
        useToastStore.getState().addToast('El recurso solicitado no existe.', 'warning');
      } else if (status >= 500) {
        useToastStore.getState().addToast('Error del servidor. Intente más tarde.', 'error');
      }
    } else if (error.request) {
      useToastStore.getState().addToast('Error de conexión. Verifique su red.', 'error');
    }

    return Promise.reject(error);
  }
);

export default apiClient;
