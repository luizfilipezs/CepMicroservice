using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Controllers;
using CepMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CepMicroservice.Tests.Controllers;

[TestClass]
public sealed class AddressControllerTests
{
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

        var mockAddressService = new Mock<IAdressService>();
        mockAddressService
            .Setup(s => s.GetByCepAsync(cep))
            .ReturnsAsync(address);

        var mockCorreiosApiService = new Mock<ICorreiosApiService>();

        var controller = new AddressController(mockAddressService.Object, mockCorreiosApiService.Object);

        // Act
        var result = await controller.GetAddress(cep);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(address, okResult.Value);
    }

    [TestMethod]
    public async Task GetAddress_ShouldReturnAddressFromApi_WhenAddressDoesNotExistInDatabase()
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

        var mockAddressService = new Mock<IAdressService>();
        mockAddressService
            .Setup(s => s.GetByCepAsync(cep))
            .ReturnsAsync(default(Address)); // Address does not exist in the database
        mockAddressService
            .Setup(s => s.SaveAsync(address))
            .Returns(Task.CompletedTask);

        var mockCorreiosApiService = new Mock<ICorreiosApiService>();
        mockCorreiosApiService
            .Setup(s => s.GetAddressByCepAsync(cep))
            .ReturnsAsync(address); // Address exists in the API

        var controller = new AddressController(mockAddressService.Object, mockCorreiosApiService.Object);

        // Act
        var result = await controller.GetAddress(cep);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(address, okResult.Value);

        // Check whether the address was saved to the database
        mockAddressService.Verify(s => s.SaveAsync(address), Times.Once);
    }

    [TestMethod]
    public async Task GetAddress_ShouldReturnNotFound_WhenAddressDoesNotExistInDatabaseOrApi()
    {
        // Arrange
        var cep = "00000000";

        var mockAddressService = new Mock<IAdressService>();
        mockAddressService
            .Setup(s => s.GetByCepAsync(cep))
            .ReturnsAsync(default(Address)); // Address does not exist in the database

        var mockCorreiosApiService = new Mock<ICorreiosApiService>();
        mockCorreiosApiService
            .Setup(s => s.GetAddressByCepAsync(cep))
            .ReturnsAsync(default(Address)); // Address does not exist in the API

        var controller = new AddressController(mockAddressService.Object, mockCorreiosApiService.Object);

        // Act
        var result = await controller.GetAddress(cep);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
}
