using System.Net.Http.Json;

namespace MeepleSystemClient.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //public async Task<bool> LoginAsync(string email, string password)
    //{
    //    try
    //    {
    //        var loginData = new
    //        {
    //            Email = email,
    //            Password = password
    //        };

    //        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"Login failed: {response.StatusCode}");
    //            return false;
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Login exception: {ex.Message}");
    //        return false;
    //    }
    //}

    private bool useMockAuth = true;

    public async Task<bool> LoginAsync(string email, string password)
    {
        if (useMockAuth)
        {
            // TEMP login
            return email == "test@test.com" && password == "password";
        }

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
        {
            Email = email,
            Password = password
        });

        return response.IsSuccessStatusCode;
    }
    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (useMockAuth)
        {
            // Fake success unless email already exists
            return email != "test@test.com";
        }

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
        {
            Email = email,
            Password = password
        });

        return response.IsSuccessStatusCode;
    }
}