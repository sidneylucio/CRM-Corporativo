import axios from 'axios';
import type {
  CustomerResponse,
  CreateCustomerCommand,
  UpdateCustomerCommand,
  CustomerEventResponse,
} from '../types/customer';

const api = axios.create({ baseURL: '/api/v1' });

export const customerApi = {
  getAll: () => api.get<CustomerResponse[]>('/customers'),
  getById: (id: string) => api.get<CustomerResponse>(`/customers/${id}`),
  create: (data: CreateCustomerCommand) =>
    api.post<CustomerResponse>('/customers', data),
  update: (id: string, data: UpdateCustomerCommand) =>
    api.put<CustomerResponse>(`/customers/${id}`, data),
  remove: (id: string) => api.delete(`/customers/${id}`),
  getEvents: (id: string) =>
    api.get<CustomerEventResponse[]>(`/customers/${id}/events`),
};
