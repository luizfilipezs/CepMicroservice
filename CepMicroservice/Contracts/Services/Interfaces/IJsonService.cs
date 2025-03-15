using System.Text.Json;

namespace CepMicroservice.Contracts.Services.Interfaces
{
    public interface IJsonService
    {
        /// <summary>
        /// Asynchronously retrieves and returns the JSON string from an HTTP response.
        /// </summary>
        /// <param name="httpResponse">The HTTP response message containing the JSON content.</param>
        /// <returns>A task representing the asynchronous operation, with a JSON string as the result.</returns>
        /// <exception cref="JsonServiceException">Thrown when the response body is not a valid JSON string.</exception>
        public Task<string> GetJsonStringFromHttpResponseAsync(HttpResponseMessage httpResponse);

        /// <summary>
        /// Determines whether the given JSON string is valid.
        /// </summary>
        /// <param name="json">The JSON string to validate.</param>
        /// <returns><see langword="true"/> if the JSON string is valid; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method checks that the string is not <see cref="string.IsNullOrWhiteSpace"/>,
        /// that it is trimmed, and that it either starts and ends with curly brackets or
        /// square brackets. If all of these conditions are met, it attempts to parse the
        /// string as a <see cref="JToken"/>. If the parsing attempt fails, the method
        /// returns <see langword="false"/>. If the parsing attempt succeeds, the method
        /// returns <see langword="true"/>.
        /// </remarks>
        public bool IsValidJson(string json);

        /// <summary>
        /// Parses the given JSON string and returns the JSON document.
        /// </summary>
        /// <param name="json">The JSON string to be parsed.</param>
        /// <returns>The JSON document parsed from the given string.</returns>
        /// <exception cref="JsonServiceException">Thrown when the given string is not a valid JSON or JSON parsing failed.</exception>
        public JsonDocument GetJsonDocumentFromString(string json);

        /// <summary>
        /// Deserializes a given <see cref="JsonElement"/> into its target type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize into.</typeparam>
        /// <param name="jsonElement">The <see cref="JsonElement"/> to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="JsonServiceException">Thrown if the deserialization fails.</exception>
        public T DeserializeJsonElementAs<T>(JsonElement jsonElement);
    }
}
