using System.Text.Json;
using System.Text.Json.Serialization;
using CRM.Corporativo.Domain.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Corporativo.Infra.Services.ViaCep;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepService> _logger;
    private const int MaxRetries = 3;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ViaCepAddress?> GetAddressAsync(string zipCode)
    {
        var digits = new string(zipCode.Where(char.IsDigit).ToArray());

        if (digits.Length != 8)
            return null;

        _logger.LogInformation("Consultando ViaCEP para o CEP {ZipCode}", digits);

        return await ExecuteWithRetryAsync(async () =>
        {
            var response = await _httpClient.GetAsync($"{digits}/json/");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ViaCepResponse>(content, JsonOptions);

            if (result?.Erro == true)
            {
                _logger.LogWarning("CEP {ZipCode} não encontrado no ViaCEP", digits);
                return null;
            }

            return result is null ? null : new ViaCepAddress(
                result.Cep,
                result.Logradouro,
                result.Complemento,
                result.Bairro,
                result.Localidade,
                result.Uf);
        });
    }

    private async Task<ViaCepAddress?> ExecuteWithRetryAsync(Func<Task<ViaCepAddress?>> action)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException ex) when (attempt < MaxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                _logger.LogWarning(ex, "Tentativa {Attempt}/{MaxRetries} falhou. Aguardando {Delay}s...", attempt, MaxRetries, delay.TotalSeconds);
                await Task.Delay(delay);
            }
        }

        return null;
    }

    private record ViaCepResponse(
        [property: JsonPropertyName("cep")] string? Cep,
        [property: JsonPropertyName("logradouro")] string? Logradouro,
        [property: JsonPropertyName("complemento")] string? Complemento,
        [property: JsonPropertyName("bairro")] string? Bairro,
        [property: JsonPropertyName("localidade")] string? Localidade,
        [property: JsonPropertyName("uf")] string? Uf,
        [property: JsonPropertyName("erro")] bool Erro = false);
}
