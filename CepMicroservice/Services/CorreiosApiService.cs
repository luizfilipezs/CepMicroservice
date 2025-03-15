using System.Text.Json;
using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Models;

namespace CepMicroservice.Services
{
    public class CorreiosApiService(HttpClient httpClient) : ICorreiosApiService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Address?> GetAddressByCepAsync(string cep)
        {
            var url = $"https://viacep.com.br/ws/{cep}/json/";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await ParseJsonResponseAsync<Address>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<T> ParseJsonResponseAsync<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new Exception("O conteúdo da resposta está vazio.");
            }

            using var jsonDocument = JsonDocument.Parse(jsonString);

            if (jsonDocument.RootElement.TryGetProperty("erro", out var errorProperty))
            {
                var errorMessage = errorProperty.GetString();
                throw new Exception($"Erro no JSON: {errorMessage}");
            }

            return jsonDocument.RootElement.Deserialize<T>() ?? throw new Exception("O objeto deserializado é nulo.");
        }
    }
}
