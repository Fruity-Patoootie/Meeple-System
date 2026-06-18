using MeepleSystemClient.Models;
using System.Net.Http.Json;

namespace MeepleSystemClient.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7093/");
        }

        // ================= LOGIN =================
        public async Task<LoginResponseModel?> LoginAsync(LoginModel model)
        {
            Console.WriteLine("Calling API: Login");

            var response = await _httpClient.PostAsJsonAsync("Login", model);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Login FAILED: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseModel>();
        }

        // ================= REGISTER (FIXED) =================
        public async Task<SaveUserResponse?> RegisterAsync(LoginModel model)
        {
            Console.WriteLine("Calling API: Register");

            var response = await _httpClient.PostAsJsonAsync("Register", model);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Register FAILED: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SaveUserResponse>();
        }
    }
}
