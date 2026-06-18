using MeepleSystemClient.ViewModels;
using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MeepleSystemClient.Pages
{
    [Authorize]
    public class CheckInModel : PageModel
    {
        // ==================== SERVICES ====================

        private readonly IGameService _gameService;

        public CheckInModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        // ==================== BARCODE INPUT ====================

        [BindProperty]
        public string CheckInBarcode { get; set; } = "";


        // ============ TITLE INPUT FOR BARCODE CHECK =============
        [BindProperty]
        public string TitleForBarcodeToCheck { get; set; } = "";


        // ==================== CHECKED IN GAMES ====================

        [BindProperty]
        public List<Game> CheckedInGames { get; set; } = new();

        // ===== BOOLEAN TO CHECK IF BARCODE ENTERED IS VALID  ======
        [BindProperty]
        public bool isValidBarcode { get; set; } = true;

        // ==================== LEGACY / FUTURE ====================

        [BindProperty]
        public int DeleteBarcode { get; set; }

        [BindProperty]
        public AddGameViewModel NewGame { get; set; } = new();

        // ==================== STATUS MESSAGE ====================

        [TempData]
        public string? SuccessMessage { get; set; }

        // =========================================================
        // LOOKUP BARCODE
        // Adds game to check-in queue
        // =========================================================

        public async Task<IActionResult> OnPostLookupBarcodeAsync()
        {
            try
            {
                // Prevent empty scans
                if (string.IsNullOrWhiteSpace(CheckInBarcode))
                    return Page();

                Console.WriteLine(
                    $"Looking up barcode: {CheckInBarcode}");

                // Call API lookup
                var result =
                    await _gameService.CheckBarcodeAsync(
                        CheckInBarcode);

                // Valid game found
                if (result?.Game != null)
                {
                    // Prevent duplicates
                    bool alreadyExists =
                        CheckedInGames.Any(g =>
                            g.Barcode == result.Game.Barcode);

                    if (!alreadyExists)
                    {
                        CheckedInGames.Add(result.Game);

                        Console.WriteLine(
                            $"Added game: {result.Game.Title}");
                    }
                }
                else
                {
                    SuccessMessage = "No game found with that barcode.";
                    isValidBarcode = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"LookupBarcode ERROR: {ex.Message}");

                SuccessMessage =
                    "Error checking barcode.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddBarcodeAsync()
        {
            try
            {
                // Prevent empty scans
                if (string.IsNullOrWhiteSpace(CheckInBarcode))
                {
                    SuccessMessage = "Please enter a valid barcode";
                    return Page();
                }

                // Prevent empty scans
                if (string.IsNullOrWhiteSpace(TitleForBarcodeToCheck))
                {
                    SuccessMessage = "Please enter a game already added";
                    return Page();
                }

                Console.WriteLine(
                    $"Looking up game by Title: {TitleForBarcodeToCheck}");

                // Call API lookup
                GameData result =await _gameService.AddBarcodeAsync(TitleForBarcodeToCheck, CheckInBarcode);

                // Valid game found
                if (result != null)
                {
                    Game gameConvert = new Game()
                    {
                        GameId = result.GameId,
                        Title = result.Title,
                        Barcode = result.Barcode,
                        Style = result.StyleName,
                        LocationName = result.LocationName,
                        ImageLocation = result.ImageLocation,
                    };
                    CheckedInGames.Add(gameConvert);

                    // setting the check method back to true
                    isValidBarcode = true;

                    Console.WriteLine(
                        $"Added game: {gameConvert.Title}");
                }
                else
                {
                    SuccessMessage = "No game found with that barcode.";
                    isValidBarcode = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"LookupBarcode ERROR: {ex.Message}");

                SuccessMessage =
                    "Error checking barcode.";
            }

            return Page();
        }

        // =========================================================
        // REMOVE GAME
        // Removes game from queue
        // =========================================================

        public IActionResult OnPostRemoveGame(string barcode)
        {
            try
            {
                CheckedInGames =
                    CheckedInGames
                        .Where(g => g.Barcode != barcode)
                        .ToList();

                Console.WriteLine(
                    $"Removed game barcode: {barcode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"RemoveGame ERROR: {ex.Message}");
            }

            return Page();
        }

        // =========================================================
        // FINAL CHECK IN
        // Sends all queued games to API
        // =========================================================

        public async Task<IActionResult> OnPostCheckInGamesAsync()
        {
            try
            {
                // Prevent empty submissions
                if (!CheckedInGames.Any())
                {
                    SuccessMessage =
                        "No games selected.";

                    return Page();
                }

                // Check in each game
                foreach (var game in CheckedInGames)
                {
                    await _gameService
                        .CheckInGameAsync(game.Title);

                    Console.WriteLine(
                        $"Checked in: {game.Title}");
                }

                SuccessMessage =
                    "Games checked in successfully!";

                // Clear queue after success
                CheckedInGames.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"CheckInGames ERROR: {ex.Message}");

                SuccessMessage =
                    "Error checking in games.";
            }

            return Page();
        }

        // =========================================================
        // LEGACY PLACEHOLDERS
        // =========================================================

        public async Task<IActionResult> OnPostAddGameAsync()
        {
            var game = new Game
            {
                Title = NewGame.Title,
                Barcode = NewGame.Barcode.ToString(),
                Style = "Unknown",
                Weight = 1.0f,
            };

            // Future implementation
            // await _gameService.AddGame(game);

            SuccessMessage =
                $"Game '{NewGame.Title}' added!";

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            // Future implementation
            // await _gameService.DeleteGame(DeleteBarcode);

            SuccessMessage =
                $"Deleted game {DeleteBarcode}";

            return Page();
        }
    }
}