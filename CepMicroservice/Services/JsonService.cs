using System.Text.Json;
using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Exceptions;
using Newtonsoft.Json.Linq;

namespace CepMicroservice.Services
{
    public class JsonService : IJsonService
    {
        public async Task<string> GetJsonStringFromHttpResponseAsync(HttpResponseMessage httpResponse)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();

            if (!IsValidJson(json))
            {
                throw new JsonServiceException("The response body is not a valid JSON string.");
            }

            return json;
        }

        public bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            json = json.Trim();

            if (!(json.StartsWith('{') && json.EndsWith('}')) && !(json.StartsWith('[') && json.EndsWith(']')))
            {
                return false;
            }

            try
            {
                var obj = JToken.Parse(json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public JsonDocument GetJsonDocumentFromString(string json)
        {
            if (!IsValidJson(json))
            {
                throw new JsonServiceException("The given string is not a valid JSON.");
            }

            try
            {
                return JsonDocument.Parse(json);
            }
            catch (Exception e)
            {
                throw new JsonServiceException("JSON parsing failed. Details: " + e.Message);
            }
        }

        public T DeserializeJsonElementAs<T>(JsonElement jsonElement)
        {
            try
            {
                var result = jsonElement.Deserialize<T>();
                return result ?? throw new JsonServiceException("JSON deserialization returned null.");
            }
            catch (JsonServiceException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new JsonServiceException("JSON deserialization failed. Details: " + e.Message);
            }
        }
    }
}
