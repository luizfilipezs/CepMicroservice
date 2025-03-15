using CepMicroservice.Data;
using CepMicroservice.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CepMicroservice.Tests.Controllers;

[TestClass]
[TestCategory("Integration")]
public sealed class AddressControllerIntegrationTests
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    [TestInitialize]
    public void Initialize()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var databaseName = Guid.NewGuid().ToString();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });
            });

        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task GetAddress_ShouldReturnAddress_WhenAddressExistsInDatabase()
    {
        // Arrange
        var cep = "12345678";
        var address = new Address
        {
            Cep = cep,
            Logradouro = "Rua Teste",
            Bairro = "Bairro Teste",
            Cidade = "Cidade Teste",
            Estado = "SP"
        };

        // Saves the address in the database
        using (var scope = _factory!.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Addresses.Add(address);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client!.GetAsync($"/api/address/{cep}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Address>();
        Assert.IsNotNull(result);
        Assert.AreEqual(cep, result.Cep);
    }

    [TestMethod]
    public async Task GetAddress_ShouldReturnAddressFromApi_WhenAddressDoesNotExistInDatabase()
    {
        // Arrange
        var cep = "01001000"; // Valid CEP for real tests (Praça da Sé, São Paulo, SP)

        // Act
        var response = await _client!.GetAsync($"/api/address/{cep}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Address>();
        Assert.IsNotNull(result);
        Assert.AreEqual(cep, result.Cep);

        // Checks whether the address was saved in the database
        using var scope = _factory!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedAddress = await context.Addresses.FirstOrDefaultAsync(a => a.Cep == cep);
        Assert.IsNotNull(savedAddress);
    }

    [TestMethod]
    public async Task GetAddress_ShouldReturnNotFound_WhenAddressDoesNotExistInDatabaseOrApi()
    {
        // Arrange
        var cep = "00000000"; // Invalid CEP

        // Act
        var response = await _client!.GetAsync($"/api/address/{cep}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
