import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { donationsApi } from '../../api/donations';
import { FormField } from '../../components/common/FormField';
import { useToastStore } from '../../store/toastStore';

const schema = z.object({
  donationId: z.string().min(1, 'Requerido'),
  evaluatedBy: z.string().min(1, 'Requerido'),
  isEligible: z.string().min(1, 'Requerido'),
  hemoglobinLevel: z.string().min(1, 'Requerido'),
  bloodPressure: z.string().min(1, 'Requerido'),
  notes: z.string().optional().or(z.literal('')),
});

export default function Evaluations() {
  const queryClient = useQueryClient();
  const addToast = useToastStore((s) => s.addToast);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    resolver: zodResolver(schema),
    defaultValues: { isEligible: 'true' },
  });

  const mutation = useMutation({
    mutationFn: (data: Record<string, string>) =>
      donationsApi.registerEvaluation({
        donationId: data.donationId,
        evaluatedBy: data.evaluatedBy,
        isEligible: data.isEligible === 'true',
        hemoglobinLevel: Number(data.hemoglobinLevel),
        bloodPressure: data.bloodPressure,
        notes: data.notes || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['donations'] });
      addToast('Evaluación registrada exitosamente.', 'success');
      reset();
    },
  });

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Registrar Evaluación</h1>

      <form onSubmit={handleSubmit((d) => mutation.mutate(d))} className="space-y-4 rounded-lg border bg-white p-6">
        <FormField label="ID de la Donación" error={errors.donationId?.message} required>
          <input {...register('donationId')} placeholder="Ingrese el ID de la donación" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Evaluado por" error={errors.evaluatedBy?.message} required>
          <input {...register('evaluatedBy')} placeholder="Nombre del evaluador" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="¿Apto?" error={errors.isEligible?.message} required>
          <select {...register('isEligible')} className="w-full rounded-lg border px-3 py-2 text-sm">
            <option value="true">Sí</option>
            <option value="false">No</option>
          </select>
        </FormField>

        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Nivel de Hemoglobina" error={errors.hemoglobinLevel?.message} required>
            <input type="number" step="0.1" {...register('hemoglobinLevel')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
          <FormField label="Presión Arterial" error={errors.bloodPressure?.message} required>
            <input {...register('bloodPressure')} placeholder="Ej: 120/80" className="w-full rounded-lg border px-3 py-2 text-sm" />
          </FormField>
        </div>

        <FormField label="Notas" error={errors.notes?.message}>
          <textarea {...register('notes')} rows={3} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <div className="flex justify-end gap-3 pt-2">
          <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-red-700 px-4 py-2 text-sm text-white hover:bg-red-800 disabled:opacity-50">
            {mutation.isPending ? 'Registrando...' : 'Registrar Evaluación'}
          </button>
        </div>
      </form>
    </div>
  );
}
