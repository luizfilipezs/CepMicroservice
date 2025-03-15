using System.Text.Json;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface IJsonService
    {
        public Task<string> GetJsonStringFromHttpResponseAsync(HttpResponseMessage httpResponse);

        public bool IsValidJson(string json);

        public JsonDocument GetJsonDocumentFromString(string json);

        public T DeserializeJsonElementAs<T>(JsonElement jsonElement);
    }
}
