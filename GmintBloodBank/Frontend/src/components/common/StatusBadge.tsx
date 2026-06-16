import clsx from 'clsx';

const statusColors: Record<string, string> = {
  available: 'bg-emerald-100 text-emerald-800',
  quarantined: 'bg-yellow-100 text-yellow-800',
  reserved: 'bg-blue-100 text-blue-800',
  transfused: 'bg-purple-100 text-purple-800',
  discarded: 'bg-gray-100 text-gray-800',
  expired: 'bg-red-100 text-red-800',
  completed: 'bg-emerald-100 text-emerald-800',
  scheduled: 'bg-blue-100 text-blue-800',
  cancelled: 'bg-gray-100 text-gray-800',
  rejected: 'bg-red-100 text-red-800',
  active: 'bg-emerald-100 text-emerald-800',
  inactive: 'bg-gray-100 text-gray-800',
  positive: 'bg-red-100 text-red-800',
  negative: 'bg-emerald-100 text-emerald-800',
  pending: 'bg-yellow-100 text-yellow-800',
  indeterminate: 'bg-orange-100 text-orange-800',
};

interface StatusBadgeProps {
  status: string;
}

export function StatusBadge({ status }: StatusBadgeProps) {
  const color = statusColors[status.toLowerCase()] ?? 'bg-gray-100 text-gray-800';
  return (
    <span className={clsx('inline-block rounded-full px-2.5 py-0.5 text-xs font-medium', color)}>
      {status}
    </span>
  );
}
