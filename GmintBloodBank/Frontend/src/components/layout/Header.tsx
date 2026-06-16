import { useAuthStore } from '../../store/authStore';

export function Header() {
  const { user, logout } = useAuthStore();

  return (
    <header className="h-14 bg-white border-b border-gray-200 flex items-center justify-between px-6">
      <div />
      <div className="flex items-center gap-4">
        <span className="text-sm text-gray-600">
          {user?.username ?? 'Usuario'}
        </span>
        <button
          onClick={logout}
          className="text-sm text-red-600 hover:text-red-800 font-medium"
        >
          Cerrar sesión
        </button>
      </div>
    </header>
  );
}
