using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollabApp.mvc.Services
{
    public interface IHttpServiceClient
    {
        Task<string> GetAsync(string endpoint);
        Task<string> PostAsync(string endpoint, string jsonContent);
    }
    public class HttpServiceClient : IHttpServiceClient
    {
        private readonly HttpClient _apiClient;
 
    public HttpServiceClient(IHttpClientFactory httpClientFactory)
    {
        _apiClient = httpClientFactory.CreateClient("Api");
    }

        public async Task<string> GetAsync(string endpoint)
        {
            var response = await _apiClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}