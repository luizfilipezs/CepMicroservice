using CepMicroservice.Models;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface IAdressService
    {
        Task<Address?> GetByCepAsync(string cep);
        Task SaveAsync(Address address);
    }
}
