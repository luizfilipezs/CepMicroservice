using System.Text.RegularExpressions;
using CepMicroservice.Data;
using CepMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace CepMicroservice.Services
{
    public class AddressService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<Address?> GetByCepAsync(string cep)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Cep == cep);
        }

        public async Task SaveAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }
    }
}
