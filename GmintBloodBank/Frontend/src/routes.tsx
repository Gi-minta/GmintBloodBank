import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from './components/layout/AppLayout';
import { useAuthStore } from './store/authStore';
import Landing from './pages/Landing';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import DonorsList from './pages/donors/DonorsList';
import DonorForm from './pages/donors/DonorForm';
import DonorDetail from './pages/donors/DonorDetail';
import DonationsList from './pages/donations/DonationsList';
import DonationForm from './pages/donations/DonationForm';
import Appointments from './pages/donations/Appointments';
import Evaluations from './pages/donations/Evaluations';
import BloodUnitsList from './pages/blood-units/BloodUnitsList';
import BloodUnitDetail from './pages/blood-units/BloodUnitDetail';
import Inventory from './pages/inventory/Inventory';
import Reports from './pages/reports/Reports';
import Tenants from './pages/admin/Tenants';
import TenantForm from './pages/admin/TenantForm';
import Licensing from './pages/admin/Licensing';
import DonationDetail from './pages/donations/DonationDetail';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

export const router = createBrowserRouter([
  { path: '/', element: <Landing /> },
  { path: '/login', element: <Login /> },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <AppLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: 'dashboard', element: <Dashboard /> },
      { path: 'donors', element: <DonorsList /> },
      { path: 'donors/new', element: <DonorForm /> },
      { path: 'donors/:id', element: <DonorDetail /> },
      { path: 'donors/:id/edit', element: <DonorForm /> },
      { path: 'donations', element: <DonationsList /> },
      { path: 'donations/new', element: <DonationForm /> },
      { path: 'donations/:id', element: <DonationDetail /> },
      { path: 'appointments', element: <Appointments /> },
      { path: 'evaluations', element: <Evaluations /> },
      { path: 'blood-units', element: <BloodUnitsList /> },
      { path: 'blood-units/lookup', element: <BloodUnitDetail /> },
      { path: 'inventory', element: <Inventory /> },
      { path: 'reports', element: <Reports /> },
      { path: 'admin/tenants', element: <Tenants /> },
      { path: 'admin/tenants/new', element: <TenantForm /> },
      { path: 'admin/tenants/:id/edit', element: <TenantForm /> },
      { path: 'admin/licensing', element: <Licensing /> },
    ],
  },
]);
