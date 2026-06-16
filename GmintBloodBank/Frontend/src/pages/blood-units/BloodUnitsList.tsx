import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { bloodUnitsApi } from '../../api/bloodUnits';
import { DataTable } from '../../components/common/DataTable';
import { StatusBadge } from '../../components/common/StatusBadge';
import { Modal } from '../../components/common/Modal';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { useToastStore } from '../../store/toastStore';
import type { Column } from '../../components/common/DataTable';
import type { BloodUnit } from '../../types';

export default function BloodUnitsList() {
  const queryClient = useQueryClient();
  const addToast = useToastStore((s) => s.addToast);
  const [showRegister, setShowRegister] = useState(false);
  const [showScreening, setShowScreening] = useState<string | null>(null);
  const [regForm, setRegForm] = useState({
    donationId: '',
    bloodTypeId: '',
    componentId: '1',
    volumeML: 450,
    collectionDate: new Date().toISOString().split('T')[0],
    expirationDate: '',
  });
  const [screeningForm, setScreeningForm] = useState({
    screeningType: '',
    result: 'PENDING',
    performedBy: '',
    notes: '',
  });

  const { data: available, isLoading } = useQuery({
    queryKey: ['blood-units-available'],
    queryFn: bloodUnitsApi.getAvailable,
  });

  const registerMutation = useMutation({
    mutationFn: () => bloodUnitsApi.register({
      ...regForm,
      collectionDate: new Date(regForm.collectionDate).toISOString(),
      expirationDate: new Date(regForm.expirationDate).toISOString(),
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['blood-units-available'] });
      queryClient.invalidateQueries({ queryKey: ['inventory'] });
      addToast('Unidad de sangre registrada.', 'success');
      setShowRegister(false);
      setRegForm({ donationId: '', bloodTypeId: '', componentId: '1', volumeML: 450, collectionDate: '', expirationDate: '' });
    },
  });

  const screeningMutation = useMutation({
    mutationFn: () => bloodUnitsApi.registerScreening(showScreening!, {
      ...screeningForm,
      bloodUnitId: showScreening!,
      screeningDate: new Date().toISOString(),
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['blood-units-available'] });
      addToast('Screening registrado.', 'success');
      setShowScreening(null);
    },
  });

  const releaseMutation = useMutation({
    mutationFn: (id: string) => bloodUnitsApi.release(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['blood-units-available'] });
      queryClient.invalidateQueries({ queryKey: ['inventory'] });
      addToast('Unidad liberada.', 'success');
    },
  });

  const discardMutation = useMutation({
    mutationFn: (id: string) => bloodUnitsApi.discard(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['blood-units-available'] });
      queryClient.invalidateQueries({ queryKey: ['inventory'] });
      addToast('Unidad descartada.', 'success');
    },
  });

  const columns: Column<BloodUnit>[] = [
    { key: 'unitCode', header: 'Código' },
    { key: 'bloodTypeCode', header: 'Tipo Sangre' },
    { key: 'componentName', header: 'Componente' },
    { key: 'volumeML', header: 'Vol. (ml)' },
    {
      key: 'collectionDate',
      header: 'Recolección',
      render: (d) => new Date(d.collectionDate).toLocaleDateString(),
    },
    {
      key: 'expirationDate',
      header: 'Vence',
      render: (d) => new Date(d.expirationDate).toLocaleDateString(),
    },
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
        <div className="flex gap-2 justify-end">
          <button onClick={() => setShowScreening(d.id)} className="text-xs text-blue-600 hover:text-blue-800">
            Screening
          </button>
          <button onClick={() => releaseMutation.mutate(d.id)} className="text-xs text-emerald-600 hover:text-emerald-800">
            Liberar
          </button>
          <button onClick={() => discardMutation.mutate(d.id)} className="text-xs text-red-600 hover:text-red-800">
            Descartar
          </button>
        </div>
      ),
    },
  ];

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Unidades de Sangre</h1>
        <button onClick={() => setShowRegister(true)} className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800">
          + Registrar Unidad
        </button>
      </div>

      <DataTable
        columns={columns}
        data={available ?? []}
        keyExtractor={(d) => d.id}
        isLoading={isLoading}
        emptyMessage="No hay unidades de sangre registradas."
      />

      <Modal isOpen={showRegister} onClose={() => setShowRegister(false)} title="Registrar Unidad de Sangre">
        <div className="space-y-3">
          {(['donationId', 'bloodTypeId'] as const).map((f) => (
            <div key={f}>
              <label className="block text-xs font-medium text-gray-600 mb-1 capitalize">{f.replace(/([A-Z])/g, ' $1')}</label>
              <input value={regForm[f]} onChange={(e) => setRegForm((p) => ({ ...p, [f]: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
          ))}
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Componente</label>
              <select value={regForm.componentId} onChange={(e) => setRegForm((p) => ({ ...p, componentId: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm">
                <option value="1">Sangre Total</option>
                <option value="2">Glóbulos Rojos</option>
                <option value="3">Plasma</option>
                <option value="4">Plaquetas</option>
              </select>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Volumen (ml)</label>
              <input type="number" value={regForm.volumeML} onChange={(e) => setRegForm((p) => ({ ...p, volumeML: +e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Fecha Recolección</label>
              <input type="date" value={regForm.collectionDate} onChange={(e) => setRegForm((p) => ({ ...p, collectionDate: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Fecha Vencimiento</label>
              <input type="date" value={regForm.expirationDate} onChange={(e) => setRegForm((p) => ({ ...p, expirationDate: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button onClick={() => setShowRegister(false)} className="rounded border px-3 py-1.5 text-sm hover:bg-gray-50">Cancelar</button>
            <button onClick={() => registerMutation.mutate()} disabled={registerMutation.isPending} className="rounded bg-red-700 px-3 py-1.5 text-sm text-white hover:bg-red-800 disabled:opacity-50">
              {registerMutation.isPending ? 'Registrando...' : 'Registrar'}
            </button>
          </div>
        </div>
      </Modal>

      <Modal isOpen={!!showScreening} onClose={() => setShowScreening(null)} title="Registrar Screening">
        <div className="space-y-3">
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Tipo de Screening</label>
            <input value={screeningForm.screeningType} onChange={(e) => setScreeningForm((p) => ({ ...p, screeningType: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" placeholder="Ej: VIH, Hepatitis B, Sífilis" />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Resultado</label>
            <select value={screeningForm.result} onChange={(e) => setScreeningForm((p) => ({ ...p, result: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm">
              <option value="PENDING">Pendiente</option>
              <option value="NEGATIVE">Negativo</option>
              <option value="POSITIVE">Positivo</option>
              <option value="INDETERMINATE">Indeterminado</option>
            </select>
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Realizado por</label>
            <input value={screeningForm.performedBy} onChange={(e) => setScreeningForm((p) => ({ ...p, performedBy: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Notas</label>
            <textarea value={screeningForm.notes} onChange={(e) => setScreeningForm((p) => ({ ...p, notes: e.target.value }))} rows={2} className="w-full rounded border px-3 py-2 text-sm" />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button onClick={() => setShowScreening(null)} className="rounded border px-3 py-1.5 text-sm hover:bg-gray-50">Cancelar</button>
            <button onClick={() => screeningMutation.mutate()} disabled={screeningMutation.isPending} className="rounded bg-red-700 px-3 py-1.5 text-sm text-white hover:bg-red-800 disabled:opacity-50">
              {screeningMutation.isPending ? 'Guardando...' : 'Guardar'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
