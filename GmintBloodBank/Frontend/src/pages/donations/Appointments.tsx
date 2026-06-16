import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { donationsApi } from '../../api/donations';
import { FormField } from '../../components/common/FormField';
import { useToastStore } from '../../store/toastStore';

const schema = z.object({
  donorId: z.string().min(1, 'Requerido'),
  bloodBankId: z.string().min(1, 'Requerido'),
  appointmentDate: z.string().min(1, 'Requerido'),
  notes: z.string().optional().or(z.literal('')),
});

type FormData = z.infer<typeof schema>;

export default function Appointments() {
  const queryClient = useQueryClient();
  const addToast = useToastStore((s) => s.addToast);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { appointmentDate: new Date().toISOString().split('T')[0] },
  });

  const mutation = useMutation({
    mutationFn: (data: FormData) => donationsApi.createAppointment(data as any),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['donations'] });
      addToast('Cita agendada exitosamente.', 'success');
      reset();
    },
  });

  return (
    <div className="mx-auto max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Agendar Cita</h1>

      <form onSubmit={handleSubmit((d) => mutation.mutate(d))} className="space-y-4 rounded-lg border bg-white p-6">
        <FormField label="ID del Donante" error={errors.donorId?.message} required>
          <input {...register('donorId')} placeholder="Ingrese el ID del donante" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="ID del Banco de Sangre" error={errors.bloodBankId?.message} required>
          <input {...register('bloodBankId')} placeholder="Ingrese el ID del banco" className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Fecha de la Cita" error={errors.appointmentDate?.message} required>
          <input type="datetime-local" {...register('appointmentDate')} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <FormField label="Notas" error={errors.notes?.message}>
          <textarea {...register('notes')} rows={3} className="w-full rounded-lg border px-3 py-2 text-sm" />
        </FormField>

        <div className="flex justify-end gap-3 pt-2">
          <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-red-700 px-4 py-2 text-sm text-white hover:bg-red-800 disabled:opacity-50">
            {mutation.isPending ? 'Agendando...' : 'Agendar Cita'}
          </button>
        </div>
      </form>
    </div>
  );
}
