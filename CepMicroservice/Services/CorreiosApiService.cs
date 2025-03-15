using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Models;

namespace CepMicroservice.Services
{
    public class CorreiosApiService : ICorreiosApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonService _jsonService;

        public CorreiosApiService(HttpClient httpClient, IJsonService jsonService)
        {
            _httpClient = httpClient;
            _jsonService = jsonService;

            _httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
        }

        public async Task<Address?> GetAddressByCepAsync(string cep)
        {
            try
            {
                return await GetAsJsonAsync<Address>(cep);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<T> GetAsJsonAsync<T>(string uri)
        {
            var response = await GetAsync<HttpResponseMessage>(uri + "/json/");
            return await ParseJsonResponse<T>(response);
        }

        private async Task<HttpResponseMessage> GetAsync<T>(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            return response;
        }

        private async Task<T> ParseJsonResponse<T>(HttpResponseMessage response)
        {
            var jsonString = await _jsonService.GetJsonStringFromHttpResponseAsync(response);
            using var jsonDocument = _jsonService.GetJsonDocumentFromString(jsonString);

            if (jsonDocument.RootElement.TryGetProperty("erro", out var errorProperty))
            {
                var errorMessage = errorProperty.GetString();
                throw new Exception("Response body has error entry: " + errorMessage);
            }

            return _jsonService.DeserializeJsonElementAs<T>(jsonDocument.RootElement);
        }
    }
}
