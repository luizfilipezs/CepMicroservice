using CepMicroservice.Data;
using CepMicroservice.Models;
using CepMicroservice.Services;
using Microsoft.EntityFrameworkCore;

namespace CepMicroservice.Tests.Services;

[TestClass]
public sealed class AddressServiceTests
{
    [TestMethod]
    public async Task GetByCepAsync_ShouldReturnAddress_WhenCepExists()
    {
        // Arrange
        var cep = "12345678";
        var address = new Address {
            Id = 1,
            Cep = cep,
            Logradouro = "Rua Teste",
            Bairro = "Bairro Teste",
            Cidade = "Cidade Teste",
            Estado = "SP"
        };

        // Configures the database context to use an in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Addresses.Add(address);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var service = new AddressService(context);

            // Act
            var result = await service.GetByCepAsync(cep);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(cep, result.Cep);
        }
    }

    [TestMethod]
    public async Task GetByCepAsync_ShouldReturnNull_WhenCepDoesNotExist()
    {
        // Arrange
        var cep = "00000000";

        // Configures the database context to use an in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new AddressService(context);

        // Act
        var result = await service.GetByCepAsync(cep);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SaveAsync_ShouldSaveAddress_WhenValidAddressIsProvided()
    {
        // Arrange
        var address = new Address
        {
            Cep = "12345678",
            Logradouro = "Rua Teste",
            Bairro = "Bairro Teste",
            Cidade = "Cidade Teste",
            Estado = "SP"
        };

        // Configura o DbContext com banco de dados em memória
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new AddressService(context);

        // Act
        await service.SaveAsync(address);

        // Assert
        var savedAddress = await context.Addresses.FirstOrDefaultAsync(a => a.Cep == address.Cep);
        Assert.IsNotNull(savedAddress);
        Assert.AreEqual(address.Cep, savedAddress.Cep);
        Assert.AreEqual(address.Logradouro, savedAddress.Logradouro);
        Assert.AreEqual(address.Bairro, savedAddress.Bairro);
        Assert.AreEqual(address.Cidade, savedAddress.Cidade);
        Assert.AreEqual(address.Estado, savedAddress.Estado);
    }
}
