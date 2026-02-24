import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { customerApi } from '../api/customerApi';
import { fetchAddressByCep } from '../api/viaCepApi';
import { CustomerType } from '../types/customer';
import type { CreateCustomerCommand, UpdateCustomerCommand } from '../types/customer';

interface FormState {
  name: string;
  document: string;
  customerType: CustomerType;
  birthDate: string;
  phone: string;
  email: string;
  zipCode: string;
  street: string;
  number: string;
  neighborhood: string;
  city: string;
  state: string;
  stateRegistration: string;
  isStateRegistrationExempt: boolean;
}

type FormFieldKey = keyof FormState;
type FieldErrors = Partial<Record<FormFieldKey, string>>;

const INITIAL: FormState = {
  name: '', document: '', customerType: CustomerType.PessoaFisica,
  birthDate: '', phone: '', email: '',
  zipCode: '', street: '', number: '', neighborhood: '', city: '', state: '',
  stateRegistration: '', isStateRegistrationExempt: false,
};

function normalizeDigits(value: string): string {
  return value.replace(/\D/g, '');
}

function mapErrorCodeToField(code?: string): FormFieldKey | null {
  if (!code) return null;
  const normalized = code.trim().toLowerCase();
  const segments = normalized.split('.');
  const lastSegment = segments[segments.length - 1];
  const map: Record<string, FormFieldKey> = {
    name: 'name',
    document: 'document',
    customertype: 'customerType',
    birthdate: 'birthDate',
    phone: 'phone',
    email: 'email',
    zipcode: 'zipCode',
    street: 'street',
    number: 'number',
    neighborhood: 'neighborhood',
    city: 'city',
    state: 'state',
    stateregistration: 'stateRegistration',
    isstateregistrationexempt: 'isStateRegistrationExempt',
    duplicateemail: 'email',
    duplicatedocument: 'document',
  };
  return map[normalized] ?? map[lastSegment] ?? null;
}

function Field({
  label,
  children,
  error,
}: {
  label: string;
  children: React.ReactNode;
  error?: string;
}) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>
      {children}
      {error && <p className="text-xs text-red-600 mt-1">{error}</p>}
    </div>
  );
}

const inputCls = 'w-full border rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500';
const inputErrorCls = `${inputCls} border-red-500 focus:ring-red-500`;
const disabledCls = `${inputCls} bg-gray-100 cursor-not-allowed`;

export default function CustomerFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [form, setForm] = useState<FormState>(INITIAL);
  const [submitting, setSubmitting] = useState(false);
  const [loadingData, setLoadingData] = useState(isEdit);
  const [cepLoading, setCepLoading] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  useEffect(() => {
    if (!id) return;
    customerApi.getById(id).then(({ data }) => {
      setForm({
        name: data.name,
        document: data.document,
        customerType: data.customerType,
        birthDate: data.birthDate.split('T')[0],
        phone: data.phone,
        email: data.email,
        zipCode: normalizeDigits(data.zipCode).slice(0, 8),
        street: data.street,
        number: data.number,
        neighborhood: data.neighborhood,
        city: data.city,
        state: data.state,
        stateRegistration: data.stateRegistration ?? '',
        isStateRegistrationExempt: data.isStateRegistrationExempt,
      });
    }).finally(() => setLoadingData(false));
  }, [id]);

  function set<K extends keyof FormState>(key: K, value: FormState[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
  }

  function getInputClass(field: FormFieldKey, disabled = false): string {
    if (disabled) return disabledCls;
    return fieldErrors[field] ? inputErrorCls : inputCls;
  }

  async function handleCepBlur() {
    if (form.zipCode.length !== 8) return;
    setCepLoading(true);
    const addr = await fetchAddressByCep(form.zipCode);
    setCepLoading(false);
    if (addr) {
      setForm((prev) => ({
        ...prev,
        street: addr.logradouro,
        neighborhood: addr.bairro,
        city: addr.localidade,
        state: addr.uf,
      }));
    }
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFieldErrors({});
    setSubmitting(true);
    try {
      const zipCode = normalizeDigits(form.zipCode).slice(0, 8);
      if (isEdit) {
        const cmd: UpdateCustomerCommand = {
          id: id!,
          name: form.name, phone: form.phone, email: form.email,
          zipCode, street: form.street, number: form.number,
          neighborhood: form.neighborhood, city: form.city, state: form.state,
          stateRegistration: form.stateRegistration || null,
          isStateRegistrationExempt: form.isStateRegistrationExempt,
        };
        await customerApi.update(id!, cmd);
      } else {
        const cmd: CreateCustomerCommand = {
          name: form.name, document: form.document,
          customerType: form.customerType,
          birthDate: new Date(form.birthDate + 'T00:00:00').toISOString(),
          phone: form.phone, email: form.email,
          zipCode, street: form.street, number: form.number,
          neighborhood: form.neighborhood, city: form.city, state: form.state,
          stateRegistration: form.stateRegistration || null,
          isStateRegistrationExempt: form.isStateRegistrationExempt,
        };
        await customerApi.create(cmd);
      }
      navigate('/customers');
    } catch (err: unknown) {
      const data = (err as { response?: { data?: unknown } })?.response?.data;
      if (data && typeof data === 'object' && Array.isArray((data as { errors?: unknown[] }).errors)) {
        const apiErrors = (data as { errors: Array<{ code?: string; message?: string }> }).errors;
        const newFieldErrors: FieldErrors = {};
        const generalErrors: string[] = [];

        for (const item of apiErrors) {
          const field = mapErrorCodeToField(item.code);
          const message = item.message ?? 'Erro de validação';
          if (field) {
            newFieldErrors[field] = message;
          } else {
            generalErrors.push(message);
          }
        }

        setFieldErrors(newFieldErrors);
        setErrors(generalErrors);
      } else if (Array.isArray(data)) {
        setErrors(data.map((e: { message?: string }) => e.message ?? String(e)));
      } else if (typeof data === 'string') {
        setErrors([data]);
      } else {
        setErrors(['Erro ao salvar. Verifique os dados e tente novamente.']);
      }
    } finally {
      setSubmitting(false);
    }
  }

  const isPJ = form.customerType === CustomerType.PessoaJuridica;

  if (loadingData) {
    return <div className="text-center py-16 text-gray-500">Carregando...</div>;
  }

  return (
    <div className="max-w-3xl mx-auto">
      <div className="flex items-center gap-3 mb-6">
        <button
          onClick={() => navigate('/customers')}
          className="text-gray-500 hover:text-gray-700 text-sm"
        >
          ← Voltar
        </button>
        <h2 className="text-2xl font-semibold text-gray-800">
          {isEdit ? 'Editar Cliente' : 'Novo Cliente'}
        </h2>
      </div>

      {errors.length > 0 && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-5">
          <ul className="list-disc list-inside space-y-1">
            {errors.map((e, i) => (
              <li key={i} className="text-sm text-red-700">{e}</li>
            ))}
          </ul>
        </div>
      )}

      <form onSubmit={handleSubmit} className="bg-white rounded-xl shadow p-6 space-y-8">
        {!isEdit && (
          <section>
            <h3 className="section-title">Tipo de Pessoa</h3>
            <div className="flex gap-8 mt-3">
              {[CustomerType.PessoaFisica, CustomerType.PessoaJuridica].map((type) => (
                <label key={type} className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="radio"
                    checked={form.customerType === type}
                    onChange={() => set('customerType', type)}
                    className="accent-blue-600"
                  />
                  <span className="text-sm font-medium">
                    {type === CustomerType.PessoaFisica ? 'Pessoa Física' : 'Pessoa Jurídica'}
                  </span>
                </label>
              ))}
            </div>
          </section>
        )}

        <section>
          <h3 className="section-title">Dados Cadastrais</h3>
          <div className="grid grid-cols-2 gap-4 mt-3">
            <div className="col-span-2">
              <Field label={isPJ ? 'Razão Social *' : 'Nome Completo *'} error={fieldErrors.name}>
                <input required type="text" value={form.name}
                  onChange={(e) => set('name', e.target.value)} className={getInputClass('name')} />
              </Field>
            </div>

            {!isEdit && (
              <Field label={isPJ ? 'CNPJ *' : 'CPF *'} error={fieldErrors.document}>
                <input required type="text" value={form.document}
                  onChange={(e) => set('document', normalizeDigits(e.target.value).slice(0, isPJ ? 14 : 11))}
                  placeholder={isPJ ? '00000000000000' : '00000000000'}
                  className={getInputClass('document')} />
              </Field>
            )}

            {isEdit && (
              <Field label={isPJ ? 'CNPJ' : 'CPF'}>
                <input type="text" value={form.document} disabled className={getInputClass('document', true)} />
              </Field>
            )}

            {!isEdit && (
              <Field label={isPJ ? 'Data de Fundação *' : 'Data de Nascimento *'} error={fieldErrors.birthDate}>
                <input required type="date" value={form.birthDate}
                  onChange={(e) => set('birthDate', e.target.value)} className={getInputClass('birthDate')} />
              </Field>
            )}

            <Field label="Telefone *" error={fieldErrors.phone}>
              <input required type="text" value={form.phone}
                onChange={(e) => set('phone', e.target.value)}
                placeholder="(11) 99999-9999" className={getInputClass('phone')} />
            </Field>

            <Field label="E-mail *" error={fieldErrors.email}>
              <input required type="email" value={form.email}
                onChange={(e) => set('email', e.target.value)} className={getInputClass('email')} />
            </Field>

            {isPJ && (
              <div className="col-span-2">
                <Field label="Inscrição Estadual" error={fieldErrors.stateRegistration}>
                  <div className="flex items-center gap-4">
                    <input type="text" value={form.stateRegistration}
                      onChange={(e) => set('stateRegistration', e.target.value)}
                      disabled={form.isStateRegistrationExempt}
                      placeholder="Número da IE"
                      className={form.isStateRegistrationExempt
                        ? getInputClass('stateRegistration', true)
                        : getInputClass('stateRegistration')} />
                    <label className="flex items-center gap-2 whitespace-nowrap cursor-pointer">
                      <input type="checkbox"
                        checked={form.isStateRegistrationExempt}
                        onChange={(e) => {
                          set('isStateRegistrationExempt', e.target.checked);
                          if (e.target.checked) set('stateRegistration', '');
                        }}
                        className="accent-blue-600" />
                      <span className="text-sm font-medium">Isento</span>
                    </label>
                  </div>
                </Field>
              </div>
            )}
          </div>
        </section>

        <section>
          <h3 className="section-title">Endereço</h3>
          <div className="grid grid-cols-3 gap-4 mt-3">
            <Field label="CEP *" error={fieldErrors.zipCode}>
              <input required type="text" value={form.zipCode}
                onChange={(e) => set('zipCode', normalizeDigits(e.target.value).slice(0, 8))}
                onBlur={handleCepBlur} maxLength={8} placeholder="00000000"
                className={getInputClass('zipCode')} />
              {cepLoading && <p className="text-xs text-blue-500 mt-1">Buscando endereço...</p>}
            </Field>

            <Field label="Número *" error={fieldErrors.number}>
              <input required type="text" value={form.number}
                onChange={(e) => set('number', e.target.value)} className={getInputClass('number')} />
            </Field>

            <Field label="Estado *" error={fieldErrors.state}>
              <input required type="text" value={form.state}
                onChange={(e) => set('state', e.target.value.toUpperCase().slice(0, 2))}
                maxLength={2} placeholder="UF" className={getInputClass('state')} />
            </Field>

            <div className="col-span-3">
              <Field label="Logradouro *" error={fieldErrors.street}>
                <input required type="text" value={form.street}
                  onChange={(e) => set('street', e.target.value)} className={getInputClass('street')} />
              </Field>
            </div>

            <div className="col-span-2">
              <Field label="Bairro *" error={fieldErrors.neighborhood}>
                <input required type="text" value={form.neighborhood}
                  onChange={(e) => set('neighborhood', e.target.value)} className={getInputClass('neighborhood')} />
              </Field>
            </div>

            <Field label="Cidade *" error={fieldErrors.city}>
              <input required type="text" value={form.city}
                onChange={(e) => set('city', e.target.value)} className={getInputClass('city')} />
            </Field>
          </div>
        </section>

        <div className="flex justify-end gap-3 pt-2 border-t">
          <button type="button" onClick={() => navigate('/customers')}
            className="px-5 py-2 border rounded-lg text-gray-600 hover:bg-gray-50 text-sm">
            Cancelar
          </button>
          <button type="submit" disabled={submitting}
            className="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium text-sm disabled:opacity-50 transition-colors">
            {submitting ? 'Salvando...' : isEdit ? 'Salvar Alterações' : 'Cadastrar Cliente'}
          </button>
        </div>
      </form>
    </div>
  );
}
