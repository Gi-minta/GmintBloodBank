import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { donorsApi } from '../../api/donors';
import { StatusBadge } from '../../components/common/StatusBadge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';

export default function DonorDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: donor, isLoading } = useQuery({
    queryKey: ['donor', id],
    queryFn: () => donorsApi.getById(id!),
    enabled: !!id,
  });

  if (isLoading) return <LoadingSpinner />;
  if (!donor) return <p className="text-gray-500">Donante no encontrado.</p>;

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">
          {donor.firstName} {donor.lastName}
        </h1>
        <button
          onClick={() => navigate(`/donors/${id}/edit`)}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm text-white hover:bg-red-800"
        >
          Editar
        </button>
      </div>

      <div className="rounded-lg border bg-white p-6 space-y-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Código</p>
            <p className="font-medium">{donor.donorCode}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Identificación</p>
            <p className="font-medium">{donor.identification}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Fecha de Nacimiento</p>
            <p className="font-medium">{new Date(donor.dateOfBirth).toLocaleDateString()}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Tipo de Sangre</p>
            <p className="font-medium">{donor.bloodTypeCode}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Género</p>
            <p className="font-medium">{donor.genderName}</p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Estado</p>
            <StatusBadge status={donor.isEligible ? 'active' : 'inactive'} />
          </div>
          {donor.email && (
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Email</p>
              <p className="font-medium">{donor.email}</p>
            </div>
          )}
          {donor.phone && (
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Teléfono</p>
              <p className="font-medium">{donor.phone}</p>
            </div>
          )}
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Última Donación</p>
            <p className="font-medium">
              {donor.lastDonationDate ? new Date(donor.lastDonationDate).toLocaleDateString() : 'N/A'}
            </p>
          </div>
          <div>
            <p className="text-xs text-gray-500 uppercase tracking-wide">Registrado</p>
            <p className="font-medium">{new Date(donor.createdAt).toLocaleDateString()}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
