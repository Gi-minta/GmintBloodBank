import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { reportsApi } from '../../api/reports';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';

export default function Reports() {
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');

  const stockQuery = useQuery({
    queryKey: ['stock-summary'],
    queryFn: reportsApi.getStockSummary,
  });

  const donationsQuery = useQuery({
    queryKey: ['donations-report', dateFrom, dateTo],
    queryFn: () => reportsApi.getDonationsReport(
      dateFrom || dateTo ? { from: dateFrom || undefined, to: dateTo || undefined } : undefined
    ),
  });

  if (stockQuery.isLoading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Reportes</h1>

      <div className="rounded-lg border bg-white p-4">
        <h2 className="font-semibold text-gray-900 mb-3">Resumen de Stock</h2>
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-gray-600 border-b">
              <th className="pb-2">Tipo Sangre</th>
              <th className="pb-2">Total</th>
              <th className="pb-2">Disponible</th>
              <th className="pb-2">Por Vencer (30d)</th>
            </tr>
          </thead>
          <tbody>
            {(stockQuery.data ?? []).map((item) => (
              <tr key={item.bloodType} className="border-b last:border-0">
                <td className="py-2 font-medium">{item.bloodType}</td>
                <td className="py-2">{item.totalUnits}</td>
                <td className="py-2 text-emerald-600 font-medium">{item.availableUnits}</td>
                <td className={item.expiringIn30Days > 0 ? 'py-2 text-amber-600' : 'py-2'}>{item.expiringIn30Days}</td>
              </tr>
            ))}
            {(stockQuery.data ?? []).length === 0 && (
              <tr><td colSpan={4} className="py-4 text-center text-gray-500">Sin datos.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="rounded-lg border bg-white p-4">
        <h2 className="font-semibold text-gray-900 mb-3">Reporte de Donaciones</h2>

        <div className="flex gap-3 mb-4">
          <input type="date" value={dateFrom} onChange={(e) => setDateFrom(e.target.value)} className="rounded border px-3 py-1.5 text-sm" />
          <input type="date" value={dateTo} onChange={(e) => setDateTo(e.target.value)} className="rounded border px-3 py-1.5 text-sm" />
        </div>

        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-gray-600 border-b">
              <th className="pb-2">Periodo</th>
              <th className="pb-2">Total</th>
              <th className="pb-2">Aptas</th>
              <th className="pb-2">Diferidas</th>
              <th className="pb-2">Vol. Promedio</th>
            </tr>
          </thead>
          <tbody>
            {donationsQuery.isLoading ? (
              <tr><td colSpan={5} className="py-4 text-center text-gray-500">Cargando...</td></tr>
            ) : (donationsQuery.data ?? []).length === 0 ? (
              <tr><td colSpan={5} className="py-4 text-center text-gray-500">Sin datos para el período seleccionado.</td></tr>
            ) : (
              (donationsQuery.data ?? []).map((item, i) => (
                <tr key={i} className="border-b last:border-0">
                  <td className="py-2">{item.period}</td>
                  <td className="py-2">{item.totalDonations}</td>
                  <td className="py-2 text-emerald-600">{item.eligible}</td>
                  <td className="py-2 text-amber-600">{item.deferred}</td>
                  <td className="py-2">{item.averageVolume} ml</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
