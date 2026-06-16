import clsx from 'clsx';
import type { ReactNode } from 'react';

interface StatCardProps {
  title: string;
  value: string | number;
  icon?: ReactNode;
  variant?: 'default' | 'warning' | 'danger' | 'success';
}

export function StatCard({ title, value, icon, variant = 'default' }: StatCardProps) {
  const colors = {
    default: 'bg-white border-gray-200',
    warning: 'bg-amber-50 border-amber-200',
    danger: 'bg-red-50 border-red-200',
    success: 'bg-emerald-50 border-emerald-200',
  };

  const valueColors = {
    default: 'text-gray-900',
    warning: 'text-amber-700',
    danger: 'text-red-700',
    success: 'text-emerald-700',
  };

  return (
    <div className={clsx('rounded-lg border p-4 shadow-xs', colors[variant])}>
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium text-gray-600">{title}</p>
        {icon && <span className="text-xl">{icon}</span>}
      </div>
      <p className={clsx('mt-2 text-2xl font-bold', valueColors[variant])}>{value}</p>
    </div>
  );
}
