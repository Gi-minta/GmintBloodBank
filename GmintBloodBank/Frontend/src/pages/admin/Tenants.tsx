import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { tenantsApi } from '../../api/tenants';
import { DataTable } from '../../components/common/DataTable';
import { StatusBadge } from '../../components/common/StatusBadge';
import type { Column } from '../../components/common/DataTable';
import type { Tenant } from '../../types';

export default function Tenants() {
  const navigate = useNavigate();

  const { data: tenants, isLoading } = useQuery({
    queryKey: ['tenants'],
    queryFn: tenantsApi.list,
  });

  const columns: Column<Tenant>[] = [
    { key: 'code', header: 'Código' },
    { key: 'name', header: 'Nombre' },
    {
      key: 'isActive',
      header: 'Estado',
      render: (d) => <StatusBadge status={d.isActive ? 'active' : 'inactive'} />,
    },
    {
      key: 'createdAt',
      header: 'Creado',
      render: (d) => new Date(d.createdAt).toLocaleDateString(),
    },
    {
      key: 'actions',
      header: '',
      className: 'px-4 py-3 text-right',
      render: (d) => (
        <button
          onClick={() => navigate(`/admin/tenants/${d.id}/edit`)}
          className="text-red-600 hover:text-red-800 text-sm font-medium"
        >
          Editar
        </button>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Inquilinos (Tenants)</h1>
        <button
          onClick={() => navigate('/admin/tenants/new')}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
        >
          + Nuevo Inquilino
        </button>
      </div>
      <DataTable
        columns={columns}
        data={tenants ?? []}
        keyExtractor={(d) => d.id}
        isLoading={isLoading}
        emptyMessage="No hay inquilinos registrados."
      />
    </div>
  );
}
