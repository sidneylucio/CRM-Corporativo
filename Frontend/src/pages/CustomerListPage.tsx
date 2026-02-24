import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { customerApi } from '../api/customerApi';
import type { CustomerResponse } from '../types/customer';
import { CustomerType } from '../types/customer';
import EventsModal from '../components/EventsModal';

function formatDocument(doc: string, type: CustomerType): string {
  if (type === CustomerType.PessoaFisica) {
    return doc.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  }
  return doc.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
}

export default function CustomerListPage() {
  const navigate = useNavigate();
  const [customers, setCustomers] = useState<CustomerResponse[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [eventsFor, setEventsFor] = useState<{ id: string; name: string } | null>(null);

  useEffect(() => {
    customerApi
      .getAll()
      .then(({ data }) => setCustomers(data))
      .finally(() => setLoading(false));
  }, []);

  async function handleDelete(customer: CustomerResponse) {
    if (!confirm(`Excluir o cliente "${customer.name}"?`)) return;
    await customerApi.remove(customer.id);
    setCustomers((prev) => prev.filter((c) => c.id !== customer.id));
  }

  const filtered = customers.filter(
    (c) =>
      c.name.toLowerCase().includes(search.toLowerCase()) ||
      c.document.includes(search.replace(/\D/g, '')) ||
      c.email.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-semibold text-gray-800">Clientes</h2>
        <button
          onClick={() => navigate('/customers/new')}
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-colors"
        >
          + Novo Cliente
        </button>
      </div>

      <input
        type="text"
        placeholder="Buscar por nome, CPF/CNPJ ou e-mail..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="w-full border rounded-lg px-4 py-2 mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500"
      />

      {loading ? (
        <div className="text-center py-16 text-gray-500">Carregando...</div>
      ) : filtered.length === 0 ? (
        <div className="text-center py-16 text-gray-500">
          {customers.length === 0
            ? 'Nenhum cliente cadastrado.'
            : 'Nenhum cliente encontrado para a busca.'}
        </div>
      ) : (
        <div className="overflow-x-auto bg-white rounded-xl shadow">
          <table className="min-w-full text-sm">
            <thead className="bg-gray-100 text-gray-500 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Nome / Razão Social</th>
                <th className="px-4 py-3 text-left">CPF / CNPJ</th>
                <th className="px-4 py-3 text-left">Tipo</th>
                <th className="px-4 py-3 text-left">E-mail</th>
                <th className="px-4 py-3 text-left">Telefone</th>
                <th className="px-4 py-3 text-center">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.map((c) => (
                <tr key={c.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{c.name}</td>
                  <td className="px-4 py-3 text-gray-600 font-mono text-xs">
                    {formatDocument(c.document, c.customerType)}
                  </td>
                  <td className="px-4 py-3">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-semibold ${
                        c.customerType === CustomerType.PessoaFisica
                          ? 'bg-green-100 text-green-700'
                          : 'bg-blue-100 text-blue-700'
                      }`}
                    >
                      {c.customerType === CustomerType.PessoaFisica ? 'PF' : 'PJ'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-gray-600">{c.email}</td>
                  <td className="px-4 py-3 text-gray-600">{c.phone}</td>
                  <td className="px-4 py-3 text-center space-x-3 whitespace-nowrap">
                    <button
                      onClick={() => navigate(`/customers/${c.id}/edit`)}
                      className="text-blue-600 hover:text-blue-800 font-medium text-xs"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => setEventsFor({ id: c.id, name: c.name })}
                      className="text-purple-600 hover:text-purple-800 font-medium text-xs"
                    >
                      Histórico
                    </button>
                    <button
                      onClick={() => handleDelete(c)}
                      className="text-red-600 hover:text-red-800 font-medium text-xs"
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {eventsFor && (
        <EventsModal
          customerId={eventsFor.id}
          customerName={eventsFor.name}
          onClose={() => setEventsFor(null)}
        />
      )}
    </div>
  );
}
