import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { donorsApi } from '../../api/donors';
import { FormField } from '../../components/common/FormField';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import type { CreateDonorRequest, UpdateDonorRequest } from '../../types';

const donorSchema = z.object({
  firstName: z.string().min(1, 'Requerido'),
  lastName: z.string().min(1, 'Requerido'),
  identification: z.string().min(1, 'Requerido'),
  dateOfBirth: z.string().min(1, 'Requerido'),
  bloodTypeId: z.string().min(1, 'Requerido'),
  genderId: z.string().min(1, 'Requerido'),
  email: z.string().email('Email inválido').optional().or(z.literal('')),
  phone: z.string().optional().or(z.literal('')),
});

type DonorFormData = z.infer<typeof donorSchema>;

const BLOOD_TYPES = [
  { id: '1', code: 'A+' }, { id: '2', code: 'A-' }, { id: '3', code: 'B+' },
  { id: '4', code: 'B-' }, { id: '5', code: 'AB+' }, { id: '6', code: 'AB-' },
  { id: '7', code: 'O+' }, { id: '8', code: 'O-' },
];

const GENDERS = [
  { id: '1', name: 'Masculino' },
  { id: '2', name: 'Femenino' },
  { id: '3', name: 'Otro' },
];

export default function DonorForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const isEdit = !!id;

  const { data: donor, isLoading } = useQuery({
    queryKey: ['donor', id],
    queryFn: () => donorsApi.getById(id!),
    enabled: isEdit,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DonorFormData>({
    resolver: zodResolver(donorSchema),
    values: donor
      ? {
          firstName: donor.firstName,
          lastName: donor.lastName,
          identification: donor.identification,
          dateOfBirth: donor.dateOfBirth.split('T')[0],
          bloodTypeId: donor.bloodTypeId,
          genderId: donor.genderId,
          email: donor.email ?? '',
          phone: donor.phone ?? '',
        }
      : undefined,
  });

  const mutation = useMutation({
    mutationFn: (data: CreateDonorRequest | UpdateDonorRequest) =>
      isEdit ? donorsApi.update(id!, data as UpdateDonorRequest) : donorsApi.create(data as CreateDonorRequest),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['donors'] });
      navigate('/donors');
    },
  });

  if (isEdit && isLoading) return <LoadingSpinner />;

  const onSubmit = (data: DonorFormData) => mutation.mutate(data as CreateDonorRequest);

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">
        {isEdit ? 'Editar Donante' : 'Nuevo Donante'}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 rounded-lg border bg-white p-6">
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Nombre" error={errors.firstName?.message} required>
            <input {...register('firstName')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
          <FormField label="Apellido" error={errors.lastName?.message} required>
            <input {...register('lastName')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
        </div>

        <FormField label="Identificación" error={errors.identification?.message} required>
          <input {...register('identification')} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Fecha de Nacimiento" error={errors.dateOfBirth?.message} required>
          <input type="date" {...register('dateOfBirth')} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Tipo de Sangre" error={errors.bloodTypeId?.message} required>
            <select {...register('bloodTypeId')} className="w-full rounded-lg border px-3 py-2 text-sm">
              <option value="">Seleccionar</option>
              {BLOOD_TYPES.map((bt) => (
                <option key={bt.id} value={bt.id}>{bt.code}</option>
              ))}
            </select>
          </FormField>
          <FormField label="Género" error={errors.genderId?.message} required>
            <select {...register('genderId')} className="w-full rounded-lg border px-3 py-2 text-sm">
              <option value="">Seleccionar</option>
              {GENDERS.map((g) => (
                <option key={g.id} value={g.id}>{g.name}</option>
              ))}
            </select>
          </FormField>
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Email" error={errors.email?.message}>
            <input type="email" {...register('email')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
          <FormField label="Teléfono" error={errors.phone?.message}>
            <input {...register('phone')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <button type="button" onClick={() => navigate('/donors')} className="rounded-lg border px-4 py-2 text-sm hover:bg-gray-50">
            Cancelar
          </button>
          <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-red-700 px-4 py-2 text-sm text-white hover:bg-red-800 disabled:opacity-50">
            {mutation.isPending ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </div>
  );
}
