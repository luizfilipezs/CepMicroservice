﻿using CepMicroservice.Contracts.Controllers.Interfaces;
using CepMicroservice.Contracts.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CepMicroservice.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController(IAdressService addressService, ICorreiosApiService correiosApiService) : ControllerBase, IAddressController
    {
        private readonly IAdressService _addressService = addressService;
        private readonly ICorreiosApiService _correiosApiService = correiosApiService;

        [HttpGet("{cep}")]
        public async Task<IActionResult> GetAddress(string cep)
        {
            var address = await _addressService.GetByCepAsync(cep);
            if (address != null) return Ok(address);

            address = await _correiosApiService.GetAddressByCepAsync(cep);
            if (address == null) return NotFound();

            await _addressService.SaveAsync(address);
            return Ok(address);
        }
    }
}
