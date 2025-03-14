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

                return await response.Content.ReadFromJsonAsync<Address>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
