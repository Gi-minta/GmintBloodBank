import { RouterProvider } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useEffect } from 'react';
import { router } from './routes';
import { authApi } from './api/auth';
import { useAuthStore } from './store/authStore';
import { ToastContainer } from './components/common/ToastContainer';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, staleTime: 30_000, refetchOnWindowFocus: false },
  },
});

function AuthBootstrap({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, setUser } = useAuthStore();

  useEffect(() => {
    if (isAuthenticated) {
      authApi.me().then(setUser).catch(() => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      });
    }
  }, [isAuthenticated, setUser]);

  return <>{children}</>;
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthBootstrap>
        <RouterProvider router={router} />
        <ToastContainer />
      </AuthBootstrap>
    </QueryClientProvider>
  );
}
