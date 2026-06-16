import { NavLink } from 'react-router-dom';
import clsx from 'clsx';

const navItems = [
  { to: '/dashboard', label: 'Dashboard', icon: '📊' },
  { to: '/donors', label: 'Donantes', icon: '👤' },
  { to: '/donations', label: 'Donaciones', icon: '🩸' },
  { to: '/appointments', label: 'Citas', icon: '📅' },
  { to: '/evaluations', label: 'Evaluaciones', icon: '📋' },
  { to: '/blood-units', label: 'Unidades de Sangre', icon: '🏥' },
  { to: '/blood-units/lookup', label: 'Buscar Unidad', icon: '🔍' },
  { to: '/inventory', label: 'Inventario', icon: '📦' },
  { to: '/reports', label: 'Reportes', icon: '📈' },
  { to: '/admin/tenants', label: 'Inquilinos', icon: '🏢' },
  { to: '/admin/licensing', label: 'Licencias', icon: '🔑' },
];

export function Sidebar() {
  return (
    <aside className="w-60 min-h-screen bg-white border-r border-gray-200 flex flex-col">
      <div className="p-4 border-b border-gray-200">
        <h1 className="text-lg font-bold text-red-700">GmintBloodBank</h1>
      </div>
      <nav className="flex-1 p-2 space-y-1">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              clsx(
                'flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors',
                isActive
                  ? 'bg-red-50 text-red-700 font-medium'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              )
            }
          >
            <span>{item.icon}</span>
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
