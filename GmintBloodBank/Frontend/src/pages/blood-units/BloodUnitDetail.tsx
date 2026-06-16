import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { bloodUnitsApi } from '../../api/bloodUnits';
import { StatusBadge } from '../../components/common/StatusBadge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';

export default function BloodUnitDetail() {
  const [code, setCode] = useState('');
  const [searchCode, setSearchCode] = useState('');

  const { data: unit, isLoading, isFetching } = useQuery({
    queryKey: ['blood-unit', searchCode],
    queryFn: () => bloodUnitsApi.getByCode(searchCode),
    enabled: searchCode.length > 0,
  });

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (code.trim()) setSearchCode(code.trim());
  };

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Buscar Unidad de Sangre</h1>

      <form onSubmit={handleSearch} className="flex gap-3">
        <input
          type="text"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          placeholder="Ingrese el código de la unidad (código de barras / QR)"
          className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-red-500"
        />
        <button
          type="submit"
          disabled={!code.trim() || isFetching}
          className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800 disabled:opacity-50"
        >
          {isFetching ? 'Buscando...' : 'Buscar'}
        </button>
      </form>

      {isLoading && <LoadingSpinner />}

      {unit && !isLoading && (
        <div className="rounded-lg border bg-white p-6 space-y-4">
          <h2 className="font-semibold text-gray-900">Detalle de la Unidad</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Código</p>
              <p className="font-medium font-mono text-sm">{unit.unitCode}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Código QR</p>
              <p className="font-medium font-mono text-xs">{unit.qrCode}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Tipo de Sangre</p>
              <p className="font-medium">{unit.bloodTypeCode}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Componente</p>
              <p className="font-medium">{unit.componentName}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Estado</p>
              <StatusBadge status={unit.statusCode} />
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Volumen</p>
              <p className="font-medium">{unit.volumeML} ml</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Fecha de Recolección</p>
              <p className="font-medium">{new Date(unit.collectionDate).toLocaleDateString()}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500 uppercase tracking-wide">Fecha de Vencimiento</p>
              <p className="font-medium">{new Date(unit.expirationDate).toLocaleDateString()}</p>
            </div>
            <div className="sm:col-span-2">
              <p className="text-xs text-gray-500 uppercase tracking-wide">Liberada</p>
              <p className="font-medium">{unit.isReleased ? 'Sí' : 'No'}</p>
            </div>
          </div>
        </div>
      )}

      {searchCode && !isLoading && !unit && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-700">
          No se encontró ninguna unidad con el código "{searchCode}".
        </div>
      )}
    </div>
  );
}
