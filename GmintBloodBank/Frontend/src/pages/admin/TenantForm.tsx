import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { tenantsApi } from '../../api/tenants';
import { FormField } from '../../components/common/FormField';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { useToastStore } from '../../store/toastStore';

const schema = z.object({
  code: z.string().min(1, 'Requerido'),
  name: z.string().min(1, 'Requerido'),
  connectionString: z.string().optional().or(z.literal('')),
});

type FormData = z.infer<typeof schema>;

export default function TenantForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const isEdit = !!id;

  const { data: tenant, isLoading } = useQuery({
    queryKey: ['tenant', id],
    queryFn: () => tenantsApi.getById(id!),
    enabled: isEdit,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    values: tenant
      ? {
          code: tenant.code,
          name: tenant.name,
          connectionString: tenant.connectionString ?? '',
        }
      : undefined,
  });

  const addToast = useToastStore((s) => s.addToast);

  const mutation = useMutation({
    mutationFn: (data: FormData) =>
      isEdit ? tenantsApi.update(id!, data) : tenantsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      addToast(isEdit ? 'Inquilino actualizado.' : 'Inquilino creado.', 'success');
      navigate('/admin/tenants');
    },
  });

  if (isEdit && isLoading) return <LoadingSpinner />;

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">
        {isEdit ? 'Editar Inquilino' : 'Nuevo Inquilino'}
      </h1>

      <form onSubmit={handleSubmit((d) => mutation.mutate(d))} className="space-y-4 rounded-lg border bg-white p-6">
        <FormField label="Código" error={errors.code?.message} required>
          <input {...register('code')} placeholder="Ej: hospital-central" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Nombre" error={errors.name?.message} required>
          <input {...register('name')} placeholder="Ej: Hospital Central" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Connection String" error={errors.connectionString?.message}>
          <input {...register('connectionString')} placeholder="Opcional" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <div className="flex justify-end gap-3 pt-2">
          <button type="button" onClick={() => navigate('/admin/tenants')} className="rounded-lg border px-4 py-2 text-sm hover:bg-gray-50">
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
