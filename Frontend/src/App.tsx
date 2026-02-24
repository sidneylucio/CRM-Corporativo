import { Routes, Route, Navigate, NavLink } from 'react-router-dom';
import CustomerListPage from './pages/CustomerListPage';
import CustomerFormPage from './pages/CustomerFormPage';

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-blue-700 text-white shadow-md">
        <div className="container mx-auto px-6 py-4 flex items-center gap-6">
          <span className="text-xl font-bold tracking-wide">CRM Corporativo</span>
          <NavLink
            to="/customers"
            className={({ isActive }) =>
              `text-sm font-medium transition-colors ${isActive ? 'text-white underline' : 'text-blue-200 hover:text-white'}`
            }
          >
            Clientes
          </NavLink>
        </div>
      </nav>

      <main className="container mx-auto px-4 py-8">
        <Routes>
          <Route path="/" element={<Navigate to="/customers" replace />} />
          <Route path="/customers" element={<CustomerListPage />} />
          <Route path="/customers/new" element={<CustomerFormPage />} />
          <Route path="/customers/:id/edit" element={<CustomerFormPage />} />
        </Routes>
      </main>
    </div>
  );
}
