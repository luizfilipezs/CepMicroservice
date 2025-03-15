using Microsoft.AspNetCore.Mvc;

namespace CepMicroservice.Contracts.Controllers.Interfaces
{
    public interface IAddressController
    {
        public Task<IActionResult> GetAddress(string cep);
    }
}
