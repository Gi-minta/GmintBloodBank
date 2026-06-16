import { Link } from 'react-router-dom';

export default function Landing() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-red-700 via-red-600 to-red-800 text-white">
      <header className="flex items-center justify-between px-8 py-4">
        <h1 className="text-2xl font-bold">GmintBloodBank</h1>
        <Link
          to="/login"
          className="rounded-lg bg-white px-5 py-2 text-sm font-semibold text-red-700 hover:bg-red-50 transition-colors"
        >
          Iniciar sesión
        </Link>
      </header>

      <main className="flex flex-col items-center justify-center px-6 pt-24 pb-32 text-center">
        <h2 className="max-w-3xl text-4xl font-bold leading-tight sm:text-5xl">
          Gestión inteligente de bancos de sangre
        </h2>
        <p className="mt-4 max-w-xl text-lg text-red-100">
          Controla donantes, donaciones, inventario de unidades sanguíneas y más, todo en un solo lugar.
        </p>
        <div className="mt-10 flex gap-4">
          <Link
            to="/login"
            className="rounded-lg bg-white px-6 py-3 font-semibold text-red-700 hover:bg-red-50 transition-colors"
          >
            Comenzar
          </Link>
        </div>

        <div className="mt-20 grid gap-8 sm:grid-cols-3">
          {[
            { icon: '🩸', title: 'Donantes', desc: 'Registro y seguimiento completo de donantes.' },
            { icon: '📦', title: 'Inventario', desc: 'Control de unidades por tipo sanguíneo y estado.' },
            { icon: '📊', title: 'Reportes', desc: 'Estadísticas de donaciones y stock disponible.' },
          ].map((f) => (
            <div key={f.title} className="rounded-xl bg-white/10 p-6 backdrop-blur-sm">
              <div className="text-3xl">{f.icon}</div>
              <h3 className="mt-3 text-lg font-semibold">{f.title}</h3>
              <p className="mt-1 text-sm text-red-100">{f.desc}</p>
            </div>
          ))}
        </div>
      </main>

      <footer className="border-t border-white/20 px-8 py-6 text-center text-sm text-red-200">
        &copy; {new Date().getFullYear()} GmintBloodBank. Todos los derechos reservados.
      </footer>
    </div>
  );
}
