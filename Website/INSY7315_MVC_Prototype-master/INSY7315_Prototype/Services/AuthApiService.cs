using INSY7315_Prototype.ViewModels;
using System.Net.Http.Json;

namespace INSY7315_Prototype.Services
{
    public class AuthApiService
    {

        //Converts the info entered into JSON for the API to read

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

        public async Task<LoginResponseViewModel?> RegisterAsync(RegisterViewModel model)
        {
            var request = new RegisterRequestViewModel
            {
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                IdNumber = model.IdNumber,
                CellNo = model.Phone
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
        }
    }
}
