using CepMicroservice.Models;

namespace CepMicroservice.Services
{
    public class CorreiosApiService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Address?> GetAddressFromCorreiosAsync(string cep)
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
