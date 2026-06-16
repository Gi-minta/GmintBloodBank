import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { donationsApi } from '../../api/donations';
import { StatusBadge } from '../../components/common/StatusBadge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';

export default function DonationDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: donation, isLoading } = useQuery({
    queryKey: ['donation', id],
    queryFn: () => donationsApi.getById(id!),
    enabled: !!id,
  });

  if (isLoading) return <LoadingSpinner />;
  if (!donation) return <p className="text-gray-500">Donación no encontrada.</p>;

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">
          Donación {donation.donationCode}
        </h1>
        <button
          onClick={() => navigate('/donations')}
          className="rounded-lg border px-4 py-2 text-sm hover:bg-gray-50"
        >
          Volver
        </button>
      </div>

      <div className="rounded-lg border bg-white p-6 space-y-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Código</p>
            <p className="font-medium">{donation.donationCode}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Donante</p>
            <p className="font-medium">{donation.donorName}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Banco de Sangre</p>
            <p className="font-medium">{donation.bloodBankName}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Estado</p>
            <StatusBadge status={donation.statusCode} />
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Fecha de Donación</p>
            <p className="font-medium">{new Date(donation.donationDate).toLocaleDateString()}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Volumen</p>
            <p className="font-medium">{donation.volumeML} ml</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Código de Bolsa</p>
            <p className="font-medium">{donation.collectionBagCode}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Registrada</p>
            <p className="font-medium">{new Date(donation.createdAt).toLocaleDateString()}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
