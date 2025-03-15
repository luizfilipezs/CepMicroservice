using Microsoft.AspNetCore.Mvc;

namespace CepMicroservice.Contracts.Controllers.Interfaces
{
    public interface IAddressController
    {
        /// <summary>
        /// Gets the address from the database or from the Correios API if not found in the database.
        /// </summary>
        /// <param name="cep">The cep to search for.</param>
        /// <returns>
        ///     <see cref="OkResult"/> if the address was found in the database or API,
        ///     <see cref="NotFoundResult"/> if the address was not found in the database or API.
        /// </returns>
        public Task<IActionResult> GetAddress(string cep);
    }
}
