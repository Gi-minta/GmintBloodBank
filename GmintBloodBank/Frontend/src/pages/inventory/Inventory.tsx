import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { inventoryApi } from '../../api/inventory';
import { Modal } from '../../components/common/Modal';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';

export default function Inventory() {
  const queryClient = useQueryClient();
  const [showMovement, setShowMovement] = useState(false);
  const [movForm, setMovForm] = useState({ bloodUnitId: '', movementType: 'TRANSFER', fromLocation: '', toLocation: '', notes: '' });

  const { data: inventory, isLoading } = useQuery({
    queryKey: ['inventory'],
    queryFn: inventoryApi.getInventory,
  });

  const { data: expiring } = useQuery({
    queryKey: ['expiring-units'],
    queryFn: inventoryApi.getExpiring,
  });

  const mutation = useMutation({
    mutationFn: () => inventoryApi.registerMovement(movForm),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventory'] });
      setShowMovement(false);
      setMovForm({ bloodUnitId: '', movementType: 'TRANSFER', fromLocation: '', toLocation: '', notes: '' });
    },
  });

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Inventario</h1>
        <button onClick={() => setShowMovement(true)} className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800">
          + Registrar Movimiento
        </button>
      </div>

      <div className="rounded-lg border bg-white p-4">
        <h2 className="font-semibold text-gray-900 mb-3">Stock por Tipo Sanguíneo</h2>
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-gray-600 border-b">
              <th className="pb-2">Tipo</th>
              <th className="pb-2">Disponible</th>
              <th className="pb-2">Cuarentena</th>
              <th className="pb-2">Reservado</th>
              <th className="pb-2">Total</th>
            </tr>
          </thead>
          <tbody>
            {(inventory ?? []).map((item) => (
              <tr key={item.bloodTypeCode} className="border-b last:border-0">
                <td className="py-2 font-medium">{item.bloodTypeCode}</td>
                <td className="py-2 text-emerald-600 font-medium">{item.available}</td>
                <td className="py-2 text-yellow-600">{item.quarantined}</td>
                <td className="py-2 text-blue-600">{item.reserved}</td>
                <td className="py-2">{item.total}</td>
              </tr>
            ))}
            {(inventory ?? []).length === 0 && (
              <tr><td colSpan={5} className="py-4 text-center text-gray-500">Sin datos de inventario.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      {expiring && expiring.length > 0 && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-4">
          <h2 className="font-semibold text-amber-800 mb-3">Unidades por Vencer</h2>
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-amber-700 border-b border-amber-200">
                <th className="pb-2">Código</th>
                <th className="pb-2">Tipo</th>
                <th className="pb-2">Vence</th>
                <th className="pb-2">Días Rest.</th>
              </tr>
            </thead>
            <tbody>
              {expiring.map((u) => (
                <tr key={u.id} className="border-b border-amber-200 last:border-0">
                  <td className="py-2">{u.unitCode}</td>
                  <td className="py-2">{u.bloodTypeCode}</td>
                  <td className="py-2">{new Date(u.expirationDate).toLocaleDateString()}</td>
                  <td className={u.daysRemaining <= 7 ? 'py-2 text-red-600 font-medium' : 'py-2 text-amber-600'}>
                    {u.daysRemaining} días
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <Modal isOpen={showMovement} onClose={() => setShowMovement(false)} title="Registrar Movimiento">
        <div className="space-y-3">
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">ID Unidad de Sangre</label>
            <input value={movForm.bloodUnitId} onChange={(e) => setMovForm((p) => ({ ...p, bloodUnitId: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Tipo de Movimiento</label>
            <select value={movForm.movementType} onChange={(e) => setMovForm((p) => ({ ...p, movementType: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm">
              <option value="ENTRY">Entrada</option>
              <option value="TRANSFER">Transferencia</option>
              <option value="RESERVE">Reserva</option>
              <option value="RELEASE">Liberación</option>
              <option value="DISCARD">Descarte</option>
            </select>
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Ubicación Origen</label>
              <input value={movForm.fromLocation} onChange={(e) => setMovForm((p) => ({ ...p, fromLocation: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-600 mb-1">Ubicación Destino</label>
              <input value={movForm.toLocation} onChange={(e) => setMovForm((p) => ({ ...p, toLocation: e.target.value }))} className="w-full rounded border px-3 py-2 text-sm" />
            </div>
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-600 mb-1">Notas</label>
            <textarea value={movForm.notes} onChange={(e) => setMovForm((p) => ({ ...p, notes: e.target.value }))} rows={2} className="w-full rounded border px-3 py-2 text-sm" />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button onClick={() => setShowMovement(false)} className="rounded border px-3 py-1.5 text-sm hover:bg-gray-50">Cancelar</button>
            <button onClick={() => mutation.mutate()} disabled={mutation.isPending} className="rounded bg-red-700 px-3 py-1.5 text-sm text-white hover:bg-red-800 disabled:opacity-50">
              {mutation.isPending ? 'Registrando...' : 'Registrar'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
