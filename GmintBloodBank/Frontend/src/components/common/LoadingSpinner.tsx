export function LoadingSpinner({ message = 'Cargando...' }: { message?: string }) {
  return (
    <div className="flex items-center justify-center py-12 text-gray-500">
      <div className="h-6 w-6 animate-spin rounded-full border-2 border-red-600 border-t-transparent mr-2" />
      {message}
    </div>
  );
}
