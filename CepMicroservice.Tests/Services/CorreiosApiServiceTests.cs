using System.Net;
using System.Text;
using System.Text.Json;
using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Exceptions;
using CepMicroservice.Models;
using CepMicroservice.Services;
using Moq;
using Moq.Protected;

namespace CepMicroservice.Tests.Services
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class CorreiosApiServiceTests
    {
        private Mock<HttpMessageHandler>? _httpMessageHandlerMock;
        private HttpClient? _httpClient;
        private Mock<IJsonService>? _jsonServiceMock;
        private CorreiosApiService? _correiosApiService;

        [TestInitialize]
        public void Initialize()
        {
            // Configura o mock do HttpMessageHandler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Cria um HttpClient com o HttpMessageHandler mockado
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            // Configura o mock do IJsonService
            _jsonServiceMock = new Mock<IJsonService>();

            // Cria a instância do CorreiosApiService com os mocks
            _correiosApiService = new CorreiosApiService(_httpClient, _jsonServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsAddress_WhenCepIsValid()
        {
            // Arrange
            var cep = "01001000";
            var expectedAddress = new Address
            {
                Cep = "01001-000",
                Logradouro = "Praça da Sé",
                Bairro = "Sé",
                Cidade = "São Paulo",
                Estado = "São Paulo"
            };
            var jsonResponse = JsonSerializer.Serialize(expectedAddress);

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            _jsonServiceMock!
                .Setup(service => service.GetJsonStringFromHttpResponseAsync(It.IsAny<HttpResponseMessage>()))
                .ReturnsAsync(jsonResponse);

            _jsonServiceMock
                .Setup(service => service.GetJsonDocumentFromString(jsonResponse))
                .Returns(JsonDocument.Parse(jsonResponse));

            _jsonServiceMock
                .Setup(service => service.DeserializeJsonElementAs<Address>(It.IsAny<JsonElement>()))
                .Returns(expectedAddress);

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAddress.Cep, result.Cep);
            Assert.AreEqual(expectedAddress.Logradouro, result.Logradouro);
            Assert.AreEqual(expectedAddress.Bairro, result.Bairro);
            Assert.AreEqual(expectedAddress.Cidade, result.Cidade);
            Assert.AreEqual(expectedAddress.Estado, result.Estado);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsNull_WhenRequestFails()
        {
            // Arrange
            var cep = "00000000";

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Simulated error"));

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsNull_WhenResponseHasErrorEntry()
        {
            // Arrange
            var cep = "00000000";
            var jsonResponse = "{\"erro\":true}";

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            _jsonServiceMock!
                .Setup(service => service.GetJsonStringFromHttpResponseAsync(It.IsAny<HttpResponseMessage>()))
                .ReturnsAsync(jsonResponse);

            _jsonServiceMock
                .Setup(service => service.GetJsonDocumentFromString(jsonResponse))
                .Returns(JsonDocument.Parse(jsonResponse));

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsNull_WhenJsonIsInvalid()
        {
            // Arrange
            var cep = "00000000";
            var invalidJson = "invalid json";

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJson, Encoding.UTF8, "application/json")
                });

            _jsonServiceMock!
                .Setup(service => service.GetJsonStringFromHttpResponseAsync(It.IsAny<HttpResponseMessage>()))
                .ThrowsAsync(new JsonServiceException("The response body is not a valid JSON string."));

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsNull_WhenJsonIsEmpty()
        {
            // Arrange
            var cep = "00000000";
            var emptyJson = "";

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(emptyJson, Encoding.UTF8, "application/json")
                });

            _jsonServiceMock!
                .Setup(service => service.GetJsonStringFromHttpResponseAsync(It.IsAny<HttpResponseMessage>()))
                .ThrowsAsync(new JsonServiceException("The response body is not a valid JSON string."));

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAddressByCepAsync_ReturnsNull_WhenDeserializationFails()
        {
            // Arrange
            var cep = "00000000";
            var jsonResponse = "{\"cep\":\"01001-000\",\"logradouro\":\"Praça da Sé\",\"bairro\":\"Sé\",\"localidade\":\"São Paulo\",\"estado\":\"São Paulo\"}";

            _httpMessageHandlerMock!
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            _jsonServiceMock!
                .Setup(service => service.GetJsonStringFromHttpResponseAsync(It.IsAny<HttpResponseMessage>()))
                .ReturnsAsync(jsonResponse);

            _jsonServiceMock
                .Setup(service => service.GetJsonDocumentFromString(jsonResponse))
                .Returns(JsonDocument.Parse(jsonResponse));

            _jsonServiceMock
                .Setup(service => service.DeserializeJsonElementAs<Address>(It.IsAny<JsonElement>()))
                .Throws(new JsonServiceException("JSON deserialization failed."));

            // Act
            var result = await _correiosApiService!.GetAddressByCepAsync(cep);

            // Assert
            Assert.IsNull(result);
        }
    }
}
