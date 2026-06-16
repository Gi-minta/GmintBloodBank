import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { donorsApi } from '../../api/donors';
import { DataTable } from '../../components/common/DataTable';
import { StatusBadge } from '../../components/common/StatusBadge';
import type { Column } from '../../components/common/DataTable';
import type { Donor } from '../../types';

export default function DonorsList() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['donors', page, search],
    queryFn: () => donorsApi.list({ page, pageSize: 20, search }),
  });

  const columns: Column<Donor>[] = [
    { key: 'donorCode', header: 'Código' },
    {
      key: 'fullName',
      header: 'Nombre',
      render: (d) => `${d.firstName} ${d.lastName}`,
    },
    { key: 'identification', header: 'Identificación' },
    { key: 'bloodTypeCode', header: 'Tipo Sangre' },
    {
      key: 'isEligible',
      header: 'Estado',
      render: (d) => (
        <StatusBadge status={d.isEligible ? 'active' : 'inactive'} />
      ),
    },
    {
      key: 'actions',
      header: '',
      className: 'px-4 py-3 text-right',
      render: (d) => (
        <button
          onClick={() => navigate(`/donors/${d.id}`)}
          className="text-red-600 hover:text-red-800 text-sm font-medium"
        >
          Ver
        </button>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Donantes</h1>
        <button
          onClick={() => navigate('/donors/new')}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800 transition-colors"
        >
          + Nuevo Donante
        </button>
      </div>

      <input
        type="text"
        placeholder="Buscar por nombre o identificación..."
        value={search}
        onChange={(e) => { setSearch(e.target.value); setPage(1); }}
        className="w-full max-w-sm rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-red-500"
      />

      <DataTable
        columns={columns}
        data={data?.items ?? []}
        keyExtractor={(d) => d.id}
        isLoading={isLoading}
        emptyMessage="No se encontraron donantes."
      />

      {data && data.totalPages > 1 && (
        <div className="flex justify-center gap-2 text-sm">
          <button
            disabled={page <= 1}
            onClick={() => setPage((p) => p - 1)}
            className="px-3 py-1 rounded border disabled:opacity-40 hover:bg-gray-100"
          >
            Anterior
          </button>
          <span className="px-3 py-1 text-gray-600">
            Página {data.page} de {data.totalPages}
          </span>
          <button
            disabled={page >= data.totalPages}
            onClick={() => setPage((p) => p + 1)}
            className="px-3 py-1 rounded border disabled:opacity-40 hover:bg-gray-100"
          >
            Siguiente
          </button>
        </div>
      )}
    </div>
  );
}
