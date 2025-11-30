using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InventoryMangmentMvc.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");

            // Forward only the InventoryAuth cookie
            var cookie = _httpContextAccessor.HttpContext?.Request.Cookies["InventoryAuth"];
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", $"InventoryAuth={cookie}");

            return client;
        }

        // GET REQUEST
        public async Task<T> GetAsync<T>(string endpoint, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {content}");

            if (string.IsNullOrWhiteSpace(content))
                return default;

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // POST REQUEST (JSON)
        public async Task<T> PostAsync<T>(string endpoint, object data, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(data);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // PUT REQUEST (JSON) - FOR UPDATES
        public async Task<T> PutAsync<T>(string endpoint, object data, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(data);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // DELETE REQUEST - WITH GENERIC RETURN TYPE
        public async Task<T> DeleteAsync<T>(string endpoint, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // DELETE REQUEST - STRING RETURN TYPE ONLY
        public async Task<string> DeleteAsync(string endpoint, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            return responseContent;
        }

        // POST REQUEST (FORM URL ENCODED)
        public async Task<T> PostFormAsync<T>(string endpoint, Dictionary<string, string> formData, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // POST REQUEST (MULTIPART FORM DATA)
        public async Task<T> PostMultipartAsync<T>(string endpoint, Dictionary<string, string> formData, string token = null)
        {
            var client = GetClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            foreach (var kvp in formData)
            {
                content.Add(new StringContent(kvp.Value), kvp.Key);
            }

            var response = await client.PostAsync(endpoint, content);

            // Store cookies from response
            if (response.Headers.Contains("Set-Cookie"))
            {
                var setCookies = response.Headers.GetValues("Set-Cookie");
                foreach (var cookie in setCookies)
                {
                    // You might need to parse and store these cookies
                }
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error {(int)response.StatusCode}: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}