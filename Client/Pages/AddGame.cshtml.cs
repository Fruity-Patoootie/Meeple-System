using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeepleSystemClient.Pages
{
    [Authorize]
    public class AddGameModel : PageModel
    {
        private readonly IGameService _gameService;

        [BindProperty]
        public Game Game { get; set; } = new Game
        {
            Title = "",
            Barcode = "",
            Style = "",
            Weight = 0,
            NumberOfTimesPlayed = 0,
            MinPlayers = 1,
            MaxPlayers = 4
        };

        [BindProperty]
        [ValidateNever]
        public string BarcodeInput { get; set; } = "";

        public GetGameResponseModel? FoundGame { get; set; }

        public string? Message { get; set; }

        public AddGameModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        // ================= CHECK BARCODE =================
        public async Task<IActionResult> OnPostCheckBarcodeAsync()
        {
            try
            {
                var result = await _gameService.CheckBarcodeAsync(BarcodeInput);

                Console.WriteLine("=== CHECK BARCODE RESULT ===");

                if (result == null)
                {
                    Console.WriteLine("Result is NULL");
                }
                else
                {
                    Console.WriteLine($"Status: {result.Status}");
                    Console.WriteLine($"Game NULL?: {result.Game == null}");

                    if (result.Game != null)
                    {
                        Console.WriteLine($"Game Title: {result.Game.Title}");
                    }
                }

                if (result != null && result.Status && result.Game != null)
                {
                    FoundGame = result;

                    Game.Title = result.Game.Title;
                    Game.Barcode = BarcodeInput;

                    Game.MinPlayers = result.Game.MinPlayers;
                    Game.MaxPlayers = result.Game.MaxPlayers;
                    Game.MinDuration = result.Game.MinDuration;
                    Game.MaxDuration = result.Game.MaxDuration;
                    Game.Weight = result.Game.Weight;

                    Message = "Game found and loaded!";
                }
                else
                {
                    Message = "Game not found. You can create it.";
                }

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Message = $"Error checking barcode: {ex.Message}";
            }

            return Page();
        }

        // ================= ADD BARCODE =================
        public async Task<IActionResult> OnPostAddBarcodeAsync()
        {
            try
            {
                var success = await _gameService.AddBarcodeAsync(Game.Title, BarcodeInput);

                Message = success == null
                    ? "Barcode added successfully!"
                    : "Failed to add barcode.";

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Message = $"Error adding barcode: {ex.Message}";
            }

            return Page();
        }

        // ================= SAVE GAME =================
        public async Task<IActionResult> OnPostSaveGameAsync()
        {
            try
            {
                // Ensure required defaults
                Game.MinPlayers = Game.MinPlayers == 0 ? 1 : Game.MinPlayers;
                Game.MaxPlayers = Game.MaxPlayers == 0 ? 4 : Game.MaxPlayers;

                var success = await _gameService.SaveGameAsync(Game);

                Message = success
                    ? "Game saved successfully!"
                    : "Failed to save game.";

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Message = $"Error saving game: {ex.Message}";
            }

            return Page();
        }
    }
}