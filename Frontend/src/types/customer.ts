export enum CustomerType {
  PessoaFisica = 1,
  PessoaJuridica = 2,
}

export interface CustomerResponse {
  id: string;
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
  stateRegistration: string | null;
  isStateRegistrationExempt: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateCustomerCommand {
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
  stateRegistration: string | null;
  isStateRegistrationExempt: boolean;
}

export interface UpdateCustomerCommand {
  id: string;
  name: string;
  phone: string;
  email: string;
  zipCode: string;
  street: string;
  number: string;
  neighborhood: string;
  city: string;
  state: string;
  stateRegistration: string | null;
  isStateRegistrationExempt: boolean;
}

export interface CustomerEventResponse {
  id: string;
  customerId: string;
  eventType: string;
  payload: string;
  occurredAt: string;
  occurredBy: string;
}
