using CepMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CepMicroservice.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController(AddressService addressService, CorreiosApiService correiosApiService) : ControllerBase
    {
        private readonly AddressService _addressService = addressService;
        private readonly CorreiosApiService _correiosApiService = correiosApiService;

        [HttpGet("{cep}")]
        public async Task<IActionResult> GetAddress(string cep)
        {
            cep = Regex.Replace(cep, @"\D", ""); // keep only digits

            var address = await _addressService.GetByCepAsync(cep);
            if (address != null) return Ok(address);

            address = await _correiosApiService.GetAddressFromCorreiosAsync(cep);
            if (address == null) return NotFound();

            await _addressService.SaveAsync(address);
            return Ok(address);
        }
    }
}
