namespace CRM.Corporativo.Domain.Services;

public interface IViaCepService
{
    Task<ViaCepAddress?> GetAddressAsync(string zipCode);
}

public record ViaCepAddress(
    string? Cep,
    string? Logradouro,
    string? Complemento,
    string? Bairro,
    string? Localidade,
    string? Uf);
