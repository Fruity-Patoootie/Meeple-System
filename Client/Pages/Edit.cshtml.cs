using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeepleSystemClient.Pages;

public class EditGameModel : PageModel
{
    private readonly IGameService _gameService;

    [BindProperty]
    public Game Game { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public EditGameModel(IGameService gameService)
    {
        _gameService = gameService;
    }

    // ==================== LOAD GAME ====================
    public async Task<IActionResult> OnGetAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            ErrorMessage = "No title provided.";
            return Page();
        }

        var game = await _gameService.GetGameByTitleAsync(title);

        if (game == null)
        {
            ErrorMessage = $"Game not found: {title}";
            return Page();
        }

        // PREVENT NULL UI ISSUES
        game.Barcode ??= "";
        game.LocationName ??= "";
        game.SupplierName ??= "";
        game.SellerName ??= "";
        game.ImageLocation ??= "";
        game.RecommendedPlayers ??= "";
        game.BestPlayers ??= "";

        Game = game;

        return Page();
    }

    // ==================== SAVE CHANGES ====================
    public async Task<IActionResult> OnPostAsync()
    {
        // DEBUGGING
        Console.WriteLine("===== EDIT GAME POST =====");

        Console.WriteLine($"Title: {Game.Title}");
        Console.WriteLine($"StyleId: {Game.StyleId}");
        Console.WriteLine($"Location: {Game.LocationName}");
        Console.WriteLine($"Supplier: {Game.SupplierName}");
        Console.WriteLine($"Seller: {Game.SellerName}");

        // CLEAN EMPTY STRINGS
        Game.Barcode =
            string.IsNullOrWhiteSpace(Game.Barcode)
                ? null
                : Game.Barcode.Trim();

        Game.LocationName =
            string.IsNullOrWhiteSpace(Game.LocationName)
                ? null
                : Game.LocationName.Trim();

        Game.SupplierName =
            string.IsNullOrWhiteSpace(Game.SupplierName)
                ? null
                : Game.SupplierName.Trim();

        Game.SellerName =
            string.IsNullOrWhiteSpace(Game.SellerName)
                ? null
                : Game.SellerName.Trim();

        Game.ImageLocation =
            string.IsNullOrWhiteSpace(Game.ImageLocation)
                ? null
                : Game.ImageLocation.Trim();

        Game.RecommendedPlayers =
            string.IsNullOrWhiteSpace(Game.RecommendedPlayers)
                ? null
                : Game.RecommendedPlayers.Trim();

        Game.BestPlayers =
            string.IsNullOrWhiteSpace(Game.BestPlayers)
                ? null
                : Game.BestPlayers.Trim();

        // ENSURE STYLE ID EXISTS
        if (Game.StyleId <= 0)
        {
            Game.StyleId = 1;
        }

        var success = await _gameService.EditGameAsync(Game);

        if (!success)
        {
            ErrorMessage = "Failed to update game.";
            return Page();
        }

        // REDIRECT BACK TO DETAILS
        return RedirectToPage(
            "/Details",
            new { title = Game.Title });
    }
}