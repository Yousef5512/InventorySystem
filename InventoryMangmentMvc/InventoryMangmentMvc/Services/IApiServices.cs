namespace InventoryMangmentMvc.Services
{

   
        public interface IApiService
        {
            Task<T> GetAsync<T>(string endpoint, string token = null);
            Task<T> PostAsync<T>(string endpoint, object data, string token = null);
            Task<T> PutAsync<T>(string endpoint, object data, string token = null);
            Task<T> DeleteAsync<T>(string endpoint, string token = null);
            Task<string> DeleteAsync(string endpoint, string token = null);
            Task<T> PostFormAsync<T>(string endpoint, Dictionary<string, string> formData, string token = null);
            Task<T> PostMultipartAsync<T>(string endpoint, Dictionary<string, string> formData, string token = null);
        }
    }
