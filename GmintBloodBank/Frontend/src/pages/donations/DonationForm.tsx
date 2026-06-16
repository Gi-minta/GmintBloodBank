import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { donationsApi } from '../../api/donations';
import { FormField } from '../../components/common/FormField';
import type { CreateDonationRequest } from '../../types';

const schema = z.object({
  donorId: z.string().min(1, 'Requerido'),
  bloodBankId: z.string().min(1, 'Requerido'),
  donationDate: z.string().min(1, 'Requerido'),
  volumeML: z.string().min(1, 'Requerido'),
  collectionBagCode: z.string().min(1, 'Requerido'),
});

export default function DonationForm() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(schema),
    defaultValues: { donationDate: new Date().toISOString().split('T')[0] },
  });

  const mutation = useMutation({
    mutationFn: (data: CreateDonationRequest) => donationsApi.register(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['donations'] });
      navigate('/donations');
    },
  });

  const onSubmit = (data: Record<string, string>) => mutation.mutate({
    ...data,
    volumeML: Number(data.volumeML),
  } as CreateDonationRequest);

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Registrar Donación</h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 rounded-lg border bg-white p-6">
        <FormField label="ID del Donante" error={errors.donorId?.message} required>
          <input {...register('donorId')} placeholder="Ingrese el ID del donante" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="ID del Banco de Sangre" error={errors.bloodBankId?.message} required>
          <input {...register('bloodBankId')} placeholder="Ingrese el ID del banco" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Fecha de Donación" error={errors.donationDate?.message} required>
          <input type="date" {...register('donationDate')} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Volumen (ml)" error={errors.volumeML?.message} required>
          <input type="number" step="1" {...register('volumeML')} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Código de Bolsa" error={errors.collectionBagCode?.message} required>
          <input {...register('collectionBagCode')} placeholder="Código de la bolsa de recolección" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <div className="flex justify-end gap-3 pt-2">
          <button type="button" onClick={() => navigate('/donations')} className="rounded-lg border px-4 py-2 text-sm hover:bg-gray-50">
            Cancelar
          </button>
          <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-red-700 px-4 py-2 text-sm text-white hover:bg-red-800 disabled:opacity-50">
            {mutation.isPending ? 'Registrando...' : 'Registrar'}
          </button>
        </div>
      </form>
    </div>
  );
}
