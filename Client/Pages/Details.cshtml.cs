using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // Required for [Authorization]

namespace MeepleSystemClient.Pages;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IGameService _gameService;

    public Game? Game { get; set; }

    public string? ErrorMessage { get; set; }

    public DetailsModel(IGameService gameService)
    {
        _gameService = gameService;
    }

    // GET: Load game details
    public async Task<IActionResult> OnGetAsync(string title)
    {
        Console.WriteLine("TITLE RECEIVED: " + title);

        if (string.IsNullOrWhiteSpace(title))
        {
            ErrorMessage = "No title provided.";
            return Page();
        }

        Game = await _gameService.GetGameByTitleAsync(title);

        if (Game == null)
        {
            ErrorMessage = $"Game not found: {title}";
        }

        return Page();
    }

    //// POST: Example action (Check In)
    //public async Task<IActionResult> OnPostCheckInAsync(int id)
    //{
    //    await _gameService.CheckInGameAsync(id);

    //    // Reload same page
    //    return RedirectToPage(new { id });
    //}
}