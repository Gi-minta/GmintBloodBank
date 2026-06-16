import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { donationsApi } from '../../api/donations';
import { DataTable } from '../../components/common/DataTable';
import { StatusBadge } from '../../components/common/StatusBadge';
import type { Column } from '../../components/common/DataTable';
import type { Donation } from '../../types';

export default function DonationsList() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ['donations', page],
    queryFn: () => donationsApi.list({ page, pageSize: 20 }),
  });

  const columns: Column<Donation>[] = [
    { key: 'donationCode', header: 'Código' },
    { key: 'donorName', header: 'Donante' },
    { key: 'bloodBankName', header: 'Banco de Sangre' },
    {
      key: 'donationDate',
      header: 'Fecha',
      render: (d) => new Date(d.donationDate).toLocaleDateString(),
    },
    { key: 'volumeML', header: 'Vol. (ml)' },
    { key: 'collectionBagCode', header: 'Bolsa' },
    {
      key: 'statusCode',
      header: 'Estado',
      render: (d) => <StatusBadge status={d.statusCode} />,
    },
    {
      key: 'actions',
      header: '',
      className: 'px-4 py-3 text-right',
      render: (d) => (
        <button
          onClick={() => navigate(`/donations/${d.id}`)}
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
        <h1 className="text-2xl font-bold text-gray-900">Donaciones</h1>
        <button
          onClick={() => navigate('/donations/new')}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
        >
          + Registrar Donación
        </button>
      </div>

      <DataTable
        columns={columns}
        data={data?.items ?? []}
        keyExtractor={(d) => d.id}
        isLoading={isLoading}
        emptyMessage="No hay donaciones registradas."
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
