using INSY7315_Prototype.ViewModels;
using System.Net.Http.Json;

namespace INSY7315_Prototype.Services
{
    public class AuthApiService
    {

        //Converts the Json returned by the API for the LoginResponseViewModel

        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
        }
    }
}
