import { useQuery } from '@tanstack/react-query';
import { reportsApi } from '../api/reports';
import { inventoryApi } from '../api/inventory';
import { StatCard } from '../components/common/StatCard';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export default function Dashboard() {
  const stockQuery = useQuery({
    queryKey: ['stock-summary'],
    queryFn: reportsApi.getStockSummary,
  });

  const inventoryQuery = useQuery({
    queryKey: ['inventory'],
    queryFn: inventoryApi.getInventory,
  });

  const expiringQuery = useQuery({
    queryKey: ['expiring-units'],
    queryFn: inventoryApi.getExpiring,
  });

  if (stockQuery.isLoading || inventoryQuery.isLoading) return <LoadingSpinner />;

  const totalAvailable = inventoryQuery.data?.reduce((s, i) => s + i.available, 0) ?? 0;
  const totalUnits = inventoryQuery.data?.reduce((s, i) => s + i.total, 0) ?? 0;
  const expiringCount = expiringQuery.data?.length ?? 0;
  const totalDonations = stockQuery.data?.reduce((s, i) => s + i.totalUnits, 0) ?? 0;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard title="Unidades Disponibles" value={totalAvailable} variant="success" icon="🩸" />
        <StatCard title="Total Unidades" value={totalUnits} variant="default" icon="📦" />
        <StatCard
          title="Por Vencer (30 días)"
          value={expiringCount}
          variant={expiringCount > 0 ? 'warning' : 'success'}
          icon="⏰"
        />
        <StatCard title="Donaciones Totales" value={totalDonations} variant="default" icon="📊" />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <h2 className="font-semibold text-gray-900 mb-3">Stock por Tipo Sanguíneo</h2>
          {inventoryQuery.data && inventoryQuery.data.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-gray-600 border-b">
                  <th className="pb-2">Tipo</th>
                  <th className="pb-2">Disponible</th>
                  <th className="pb-2">En Cuarentena</th>
                  <th className="pb-2">Reservado</th>
                </tr>
              </thead>
              <tbody>
                {inventoryQuery.data.map((item) => (
                  <tr key={item.bloodTypeCode} className="border-b last:border-0">
                    <td className="py-2 font-medium">{item.bloodTypeCode}</td>
                    <td className="py-2 text-emerald-600 font-medium">{item.available}</td>
                    <td className="py-2 text-yellow-600">{item.quarantined}</td>
                    <td className="py-2 text-blue-600">{item.reserved}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="text-gray-500 text-sm">Sin datos de inventario.</p>
          )}
        </div>

        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <h2 className="font-semibold text-gray-900 mb-3">Unidades por Vencer</h2>
          {expiringQuery.data && expiringQuery.data.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-gray-600 border-b">
                  <th className="pb-2">Código</th>
                  <th className="pb-2">Tipo</th>
                  <th className="pb-2">Días Rest.</th>
                </tr>
              </thead>
              <tbody>
                {expiringQuery.data.map((u) => (
                  <tr key={u.id} className="border-b last:border-0">
                    <td className="py-2">{u.unitCode}</td>
                    <td className="py-2">{u.bloodTypeCode}</td>
                    <td className={u.daysRemaining <= 7 ? 'py-2 text-red-600 font-medium' : 'py-2 text-amber-600'}>
                      {u.daysRemaining} días
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="text-gray-500 text-sm">No hay unidades por vencer.</p>
          )}
        </div>
      </div>
    </div>
  );
}
