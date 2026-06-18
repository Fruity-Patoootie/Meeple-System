using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;  // Required for cookie authentication (real login system)
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// LOGIN PAGE MODEL
// 1. Displays the Login Page
// 2. Accepts user input for email and password
// 3. Calls the API to validate user credentials
// 4. Stores the logged-in user (NOW via cookie authentication)
// 5. Redirects to the Index page when the login succeeds

namespace MeepleSystemClient.Pages;

// Allows access without being logged in (prevents redirect loop)
[AllowAnonymous]

// Razor Page Model for the Login Page
// Class name should match Login.cshtml
public class LoginModel : PageModel
{
    // Dependency injection for the user service
    private readonly IUserService _userService;

    // Constructor to inject the user service
    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    // Properties to bind the email and password input fields
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    // GET request handler for the login page
    public IActionResult OnGet()
    {
        // Check authentication using cookie system (NOT session)
        // If user is already logged in, skip login page
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    // POST request handler for the login form submission
    public async Task<IActionResult> OnPostAsync()
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return Page();
        }

        try
        {
            // Map UI → API model
            var model = new MeepleSystemClient.Models.LoginModel
            {
                Username = Email,   // API expects Username
                Password = Password
            };

            // Call API to validate credentials
            var response = await _userService.LoginAsync(model);

            if (response == null || !response.Status)
            {
                ErrorMessage = response?.Message ?? "Invalid login.";
                return Page();
            }

            // ========================= AUTHENTICATION =========================

            // Create claims (represents the logged-in user identity)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            // Create identity using the same scheme as Program.cs ("CookieAuth")
            var identity = new ClaimsIdentity(claims, "CookieAuth");

            // Create principal (this is the actual "logged-in user")
            var principal = new ClaimsPrincipal(identity);

            // This creates the authentication cookie and logs the user in
            await HttpContext.SignInAsync("CookieAuth", principal);

            // ========================= OPTIONAL SESSION STORAGE =========================
            // You can still store extra info in session if needed
            HttpContext.Session.SetString("User", model.Username);

            if (response.Data != null)
            {
                HttpContext.Session.SetInt32("UserId", response.Data.UserId);
            }

            // Redirect after successful login
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}