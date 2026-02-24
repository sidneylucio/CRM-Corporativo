import { useEffect, useState } from 'react';
import { customerApi } from '../api/customerApi';
import type { CustomerEventResponse } from '../types/customer';

interface Props {
  customerId: string;
  customerName: string;
  onClose: () => void;
}

const EVENT_LABELS: Record<string, string> = {
  CustomerCreated: 'Cliente Criado',
  CustomerUpdated: 'Cliente Atualizado',
  CustomerDeleted: 'Cliente Excluído',
};

function tryParseJson(payload: string): string {
  try {
    return JSON.stringify(JSON.parse(payload), null, 2);
  } catch {
    return payload;
  }
}

export default function EventsModal({ customerId, customerName, onClose }: Props) {
  const [events, setEvents] = useState<CustomerEventResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    customerApi
      .getEvents(customerId)
      .then(({ data }) => setEvents(data))
      .catch(() => setError('Erro ao carregar histórico.'))
      .finally(() => setLoading(false));
  }, [customerId]);

  return (
    <div
      className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
      onClick={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-2xl max-h-[80vh] flex flex-col">
        <div className="flex items-center justify-between px-6 py-4 border-b shrink-0">
          <div>
            <h3 className="text-lg font-semibold text-gray-800">Histórico de Alterações</h3>
            <p className="text-sm text-gray-500">{customerName}</p>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 text-2xl leading-none font-light"
          >
            &times;
          </button>
        </div>

        <div className="overflow-y-auto flex-1 p-6">
          {loading && <p className="text-center text-gray-500 py-8">Carregando...</p>}
          {error && <p className="text-center text-red-500 py-8">{error}</p>}
          {!loading && !error && events.length === 0 && (
            <p className="text-center text-gray-500 py-8">Nenhum evento registrado.</p>
          )}
          {!loading && events.length > 0 && (
            <ol className="relative border-l-2 border-blue-100 space-y-6 pl-6">
              {events.map((ev) => (
                <li key={ev.id} className="relative">
                  <div className="absolute -left-[1.65rem] top-1 w-3 h-3 bg-blue-500 rounded-full ring-2 ring-white" />
                  <div className="flex flex-wrap items-baseline gap-2 mb-2">
                    <span className="text-sm font-semibold text-blue-700">
                      {EVENT_LABELS[ev.eventType] ?? ev.eventType}
                    </span>
                    <span className="text-xs text-gray-400">
                      {new Date(ev.occurredAt).toLocaleString('pt-BR')}
                    </span>
                    {ev.occurredBy && (
                      <span className="text-xs text-gray-500 italic">por {ev.occurredBy}</span>
                    )}
                  </div>
                  <pre className="text-xs bg-gray-50 border rounded-lg p-3 overflow-x-auto text-gray-600 whitespace-pre-wrap break-words">
                    {tryParseJson(ev.payload)}
                  </pre>
                </li>
              ))}
            </ol>
          )}
        </div>

        <div className="px-6 py-3 border-t shrink-0 text-right">
          <button
            onClick={onClose}
            className="px-4 py-2 text-sm border rounded-lg text-gray-600 hover:bg-gray-50"
          >
            Fechar
          </button>
        </div>
      </div>
    </div>
  );
}
