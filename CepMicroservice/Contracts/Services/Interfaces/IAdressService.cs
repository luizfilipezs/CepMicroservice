using CepMicroservice.Models;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface IAdressService
    {
        /// <summary>
        /// Gets the address by cep asynchronously.
        /// </summary>
        /// <param name="cep">The cep.</param>
        /// <returns>The address, or null if none is found.</returns>
        Task<Address?> GetByCepAsync(string cep);


        /// <summary>
        /// Asynchronously saves the specified address to the database after sanitizing its CEP.
        /// </summary>
        /// <param name="address">The address to be saved, with its CEP to be sanitized.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveAsync(Address address);
    }
}
