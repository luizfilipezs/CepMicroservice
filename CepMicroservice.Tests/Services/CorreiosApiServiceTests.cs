using CepMicroservice.Models;
using CepMicroservice.Services;
using Moq;
using Moq.Protected;
using System.Net;

namespace CepMicroservice.Tests.Services;

[TestClass]
public sealed class CorreiosApiServiceTests
{
    [TestMethod]
    public async Task GetAddressByCepAsync_ShouldReturnAddress_WhenCepIsValid()
    {
        // Arrange
        var cep = "12345678";
        var expectedAddress = new Address
        {
            Cep = cep,
            Logradouro = "Rua Teste",
            Bairro = "Bairro Teste",
            Cidade = "Cidade Teste",
            Estado = "SP"
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedAddress))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new CorreiosApiService(httpClient);

        // Act
        var result = await service.GetAddressByCepAsync(cep);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedAddress.Cep, result.Cep);
        Assert.AreEqual(expectedAddress.Logradouro, result.Logradouro);
        Assert.AreEqual(expectedAddress.Bairro, result.Bairro);
        Assert.AreEqual(expectedAddress.Cidade, result.Cidade);
        Assert.AreEqual(expectedAddress.Estado, result.Estado);
    }

    [TestMethod]
    public async Task GetAddressByCepAsync_ShouldReturnNull_WhenCepIsInvalid()
    {
        // Arrange
        var cep = "00000000";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new CorreiosApiService(httpClient);

        // Act
        var result = await service.GetAddressByCepAsync(cep);

        // Assert
        Assert.IsNull(result);
    }
}
