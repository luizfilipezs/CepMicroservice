using System.Text.RegularExpressions;
using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Data;
using CepMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace CepMicroservice.Services
{
    public partial class AddressService(AppDbContext context) : IAdressService
    {
        private readonly AppDbContext _context = context;

        public async Task<Address?> GetByCepAsync(string cep)
        {
            cep = SanitizeCep(cep);
    
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Cep == cep);
        }

        public async Task SaveAsync(Address address)
        {
            address.Cep = SanitizeCep(address.Cep);

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        private static string SanitizeCep(string cep) => KeepOnlyDigitsRegex().Replace(cep, "");

        [GeneratedRegex(@"\D")]
        private static partial Regex KeepOnlyDigitsRegex();
    }
}
