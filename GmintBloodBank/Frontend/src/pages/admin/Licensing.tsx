import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { licensingApi } from '../../api/licensing';
import { Modal } from '../../components/common/Modal';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { useToastStore } from '../../store/toastStore';

export default function Licensing() {
  const queryClient = useQueryClient();
  const addToast = useToastStore((s) => s.addToast);
  const [showAssign, setShowAssign] = useState(false);
  const [assignForm, setAssignForm] = useState({ tenantId: '', licensePlanId: '' });

  const statusQuery = useQuery({
    queryKey: ['license-status'],
    queryFn: licensingApi.getStatus,
  });

  const featuresQuery = useQuery({
    queryKey: ['license-features'],
    queryFn: licensingApi.getFeatures,
  });

  const toggleMutation = useMutation({
    mutationFn: (id: string) => licensingApi.toggleFeature(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['license-features'] });
      addToast('Feature flag actualizado.', 'success');
    },
  });

  const assignMutation = useMutation({
    mutationFn: () => licensingApi.assignLicense(assignForm),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['license-status'] });
      addToast('Licencia asignada exitosamente.', 'success');
      setShowAssign(false);
      setAssignForm({ tenantId: '', licensePlanId: '' });
    },
  });

  if (statusQuery.isLoading) return <LoadingSpinner />;

  const status = statusQuery.data;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Licencias</h1>
        <button
          onClick={() => setShowAssign(true)}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
        >
          + Asignar Licencia
        </button>
      </div>

      {status && (
        <div className="rounded-lg border bg-white p-4 space-y-3">
          <h2 className="font-semibold text-gray-900">Estado de Licencia</h2>
          <div className="grid gap-3 sm:grid-cols-2">
            <div>
              <p className="text-xs text-gray-500 uppercase">Inquilino</p>
              <p className="font-medium">{status.tenantName}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase">Plan</p>
              <p className="font-medium">{status.planName}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase">Válida desde</p>
              <p className="font-medium">{new Date(status.startDate).toLocaleDateString()}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase">Vence</p>
              <p className="font-medium">{new Date(status.expirationDate).toLocaleDateString()}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase">Días restantes</p>
              <p className={`font-medium ${status.daysRemaining <= 30 ? 'text-red-600' : 'text-emerald-600'}`}>
                {status.daysRemaining} días
              </p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase">Estado</p>
              <p className={`font-medium ${status.isActive ? 'text-emerald-600' : 'text-red-600'}`}>
                {status.isActive ? 'Activa' : 'Inactiva'}
              </p>
            </div>
          </div>
        </div>
      )}

      <div className="rounded-lg border bg-white p-4">
        <h2 className="font-semibold text-gray-900 mb-3">Feature Flags</h2>
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-gray-600 border-b">
              <th className="pb-2">Clave</th>
              <th className="pb-2">Descripción</th>
              <th className="pb-2">Alcance</th>
              <th className="pb-2">Estado</th>
              <th className="pb-2"></th>
            </tr>
          </thead>
          <tbody>
            {(featuresQuery.data ?? []).map((f) => (
              <tr key={f.id} className="border-b last:border-0">
                <td className="py-2 font-mono text-xs">{f.key}</td>
                <td className="py-2">{f.description}</td>
                <td className="py-2">{f.scope}</td>
                <td className="py-2">
                  <span className={`inline-block rounded-full px-2 py-0.5 text-xs font-medium ${f.isEnabled ? 'bg-emerald-100 text-emerald-800' : 'bg-gray-100 text-gray-800'}`}>
                    {f.isEnabled ? 'Activado' : 'Desactivado'}
                  </span>
                </td>
                <td className="py-2">
                  <button
                    onClick={() => toggleMutation.mutate(f.id)}
                    className="text-xs text-red-600 hover:text-red-800"
                  >
                    Alternar
                  </button>
                </td>
              </tr>
            ))}
            {(featuresQuery.data ?? []).length === 0 && (
              <tr><td colSpan={5} className="py-4 text-center text-gray-500">Sin features registrados.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal isOpen={showAssign} onClose={() => setShowAssign(false)} title="Asignar Licencia">
        <div className="space-y-3">
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">ID del Inquilino</label>
            <input
              value={assignForm.tenantId}
              onChange={(e) => setAssignForm((p) => ({ ...p, tenantId: e.target.value }))}
              className="w-full rounded border px-3 py-2 text-sm"
              placeholder="Ingrese el ID del inquilino"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">ID del Plan de Licencia</label>
            <input
              value={assignForm.licensePlanId}
              onChange={(e) => setAssignForm((p) => ({ ...p, licensePlanId: e.target.value }))}
              className="w-full rounded border px-3 py-2 text-sm"
              placeholder="Ingrese el ID del plan"
            />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button onClick={() => setShowAssign(false)} className="rounded border px-3 py-1.5 text-sm hover:bg-gray-50">
              Cancelar
            </button>
            <button
              onClick={() => assignMutation.mutate()}
              disabled={assignMutation.isPending}
              className="rounded bg-red-700 px-3 py-1.5 text-sm text-white hover:bg-red-800 disabled:opacity-50"
            >
              {assignMutation.isPending ? 'Asignando...' : 'Asignar'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
