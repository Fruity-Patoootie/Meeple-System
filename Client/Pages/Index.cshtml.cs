using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace MeepleSystemClient.Pages
{
    // 🔐 This ensures ONLY logged-in users can access this page
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IGameService _gameService;

        // Holds game data for the page
        public List<Game> Games { get; set; } = new();

        // Stores error messages
        public string? ErrorMessage { get; set; }

        // Username shown in UI
        public string Username { get; set; } = "Guest";

        public IndexModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        // Runs when page is accessed
        public async Task<IActionResult> OnGetAsync()
        {
            // ✅ Use cookie authentication (NOT session)
            var user = User.Identity?.Name;

            // Extra safety check (rarely needed because [Authorize] already handles this)
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToPage("/Login");
            }

            // Display username (before @ symbol)
            Username = user.Split('@')[0];

            try
            {
                // Load game data
                Games = await _gameService.GetAllGamesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        // 🔐 Proper logout (clears cookie + session)
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Sign out of cookie authentication
            await HttpContext.SignOutAsync("CookieAuth");

            // Clear session (optional cleanup)
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}