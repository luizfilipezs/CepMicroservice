using CepMicroservice.Models;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface ICorreiosApiService
    {
        /// <summary>
        /// Gets the address by CEP asynchronously.
        /// </summary>
        /// <param name="cep">The cep.</param>
        /// <returns>
        /// The address by cep asynchronously.
        /// </returns>
        public Task<Address?> GetAddressByCepAsync(string cep);
    }
}
