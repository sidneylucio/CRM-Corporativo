import axios from 'axios';

export interface ViaCepAddress {
  logradouro: string;
  bairro: string;
  localidade: string;
  uf: string;
}

export async function fetchAddressByCep(cep: string): Promise<ViaCepAddress | null> {
  const clean = cep.replace(/\D/g, '');
  if (clean.length !== 8) return null;
  try {
    const { data } = await axios.get<ViaCepAddress & { erro?: boolean }>(
      `https://viacep.com.br/ws/${clean}/json/`
    );
    if (data.erro) return null;
    return data;
  } catch {
    return null;
  }
}
