using System.Net;
using System.Text;
using System.Text.Json;
using CepMicroservice.Exceptions;
using CepMicroservice.Services;
using System.Text.Json.Serialization;

namespace CepMicroservice.Tests.Services
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class JsonServiceTests
    {
        private readonly JsonService _jsonService;

        public JsonServiceTests()
        {
            _jsonService = new JsonService();
        }

        [TestMethod]
        public async Task GetJsonStringFromHttpResponseAsync_ReturnsJsonString_WhenResponseIsValid()
        {
            // Arrange
            var expectedJson = "{\"key\":\"value\"}";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };

            // Act
            var result = await _jsonService.GetJsonStringFromHttpResponseAsync(httpResponse);

            // Assert
            Assert.AreEqual(expectedJson, result);
        }

        [TestMethod]
        public async Task GetJsonStringFromHttpResponseAsync_ThrowsJsonServiceException_WhenResponseIsNotValidJson()
        {
            // Arrange
            var invalidJson = "invalid json";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(invalidJson, Encoding.UTF8, "application/json")
            };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<JsonServiceException>(() =>
                _jsonService.GetJsonStringFromHttpResponseAsync(httpResponse));
        }

        [TestMethod]
        public void IsValidJson_ReturnsTrue_WhenJsonIsValid()
        {
            // Arrange
            var validJson = "{\"key\":\"value\"}";

            // Act
            var result = _jsonService.IsValidJson(validJson);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidJson_ReturnsFalse_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "invalid json";

            // Act
            var result = _jsonService.IsValidJson(invalidJson);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidJson_ReturnsFalse_WhenJsonIsEmpty()
        {
            // Arrange
            var emptyJson = "";

            // Act
            var result = _jsonService.IsValidJson(emptyJson);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetJsonDocumentFromString_ReturnsJsonDocument_WhenJsonIsValid()
        {
            // Arrange
            var validJson = "{\"key\":\"value\"}";

            // Act
            var jsonDocument = _jsonService.GetJsonDocumentFromString(validJson);

            // Assert
            Assert.IsNotNull(jsonDocument);
            Assert.IsTrue(jsonDocument.RootElement.TryGetProperty("key", out var property));
            Assert.AreEqual("value", property.GetString());
        }

        [TestMethod]
        public void GetJsonDocumentFromString_ThrowsJsonServiceException_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "invalid json";

            // Act & Assert
            Assert.ThrowsException<JsonServiceException>(() =>
                _jsonService.GetJsonDocumentFromString(invalidJson));
        }

        [TestMethod]
        public void DeserializeJsonElementAs_ReturnsDeserializedObject_WhenJsonIsValid()
        {
            // Arrange
            var json = "{\"name\":\"John\",\"age\":30}";
            var jsonDocument = JsonDocument.Parse(json);
            var jsonElement = jsonDocument.RootElement;

            // Act
            var result = _jsonService.DeserializeJsonElementAs<Person>(jsonElement);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.Name);
            Assert.AreEqual(30, result.Age);
        }

        [TestMethod]
        public void DeserializeJsonElementAs_ThrowsJsonServiceException_WhenDeserializationFails()
        {
            // Arrange
            var invalidJson = "{\"name\":\"John\",\"age\":\"invalid\"}"; // Age should be an integer
            var jsonDocument = JsonDocument.Parse(invalidJson);
            var jsonElement = jsonDocument.RootElement;

            // Act & Assert
            var exception = Assert.ThrowsException<JsonServiceException>(() =>
                _jsonService.DeserializeJsonElementAs<Person>(jsonElement));
            Assert.IsTrue(exception.Message.Contains("JSON deserialization failed."));
        }

        // Classe auxiliar para testes de desserialização
        private class Person
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("age")]
            public int Age { get; set; }
        }
    }
}
