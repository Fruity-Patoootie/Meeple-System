using MeepleSystemClient.Models;

// This file defines a contract (interface) for user-related operations.

namespace MeepleSystemClient.Services
{
    // The IUserService interface defines methods for user authentication and registration.
    public interface IUserService
    {
        // User Login Method Interface
        Task<LoginResponseModel?> LoginAsync(LoginModel model);
        // User Registration Method Interface
        Task<SaveUserResponse?> RegisterAsync(LoginModel model);
    }
}