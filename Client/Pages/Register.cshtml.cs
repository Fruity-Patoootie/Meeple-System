using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // Required for [AllowAnonymous]

namespace MeepleSystemClient.Pages;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validation
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "All fields are required.";
            return Page();
        }

        try
        {
            // Build API model
            var model = new MeepleSystemClient.Models.LoginModel
            {
                Username = Email,   // ⚠️ API expects Username
                Password = Password
            };

            // Call API
            var response = await _userService.RegisterAsync(model);

            if (response == null || !response.Status)
            {
                ErrorMessage = response?.Message ?? "Registration failed.";
                return Page();
            }

            // Success → redirect to login
            return RedirectToPage("/Login");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}