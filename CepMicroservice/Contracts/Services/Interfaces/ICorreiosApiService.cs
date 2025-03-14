using CepMicroservice.Models;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface ICorreiosApiService
    {
        public Task<Address?> GetAddressByCepAsync(string cep);
    }
}
