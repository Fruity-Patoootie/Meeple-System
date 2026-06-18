using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Net.Mime.MediaTypeNames;
using MeepleSystemClient.Models;
using System.Text.Json;

namespace MeepleSystemClient.Pages
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class DeleteGameModel : PageModel
    {
        private readonly IGameService _gameService;
        private const string SESSION_KEY = "DELETE_QUEUE";

        public DeleteGameModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        // ==================== BARCODE INPUT ====================
        // Barcode entered/scanned by the user
        [BindProperty]
        public string Barcode { get; set; } = "";

        // ==================== TITLE INPUT ====================
        // Title entered by the user
        [BindProperty]
        public string Title { get; set; } = "";

        // ==================== SELECTED GAMES ====================
        // Stores games that are queued for deletion confirmation
        [BindProperty]
        public List<Game> GamesToDelete { get; set; } = new();

        // ==================== UI STATE ====================
        // Controls whether confirmation modal is shown
        [BindProperty]
        public bool ShowConfirmation { get; set; }

        // Success or failure messages displayed to user
        [TempData]
        public string? SuccessMessage { get; set; }

        // ==================== ADD GAME TO DELETE LIST ====================
        // Looks up a barcode and adds the game to the delete queue
        public async Task<IActionResult> OnPostLookupBarcodeAsync()
        {
            // Prevent empty barcode searches
            if (string.IsNullOrWhiteSpace(Barcode))
                return Page();

            try
            {
                // Clean barcode input
                var cleanBarcode = Barcode.Trim();

                // Call API lookup
                var result = await _gameService
                    .CheckBarcodeAsync(cleanBarcode);

                // Ensure game exists
                if (result != null &&
                    result.Status &&
                    result.Game != null)
                {
                    // Prevent duplicates from being added
                    bool alreadyExists = GamesToDelete.Any(g =>
                        g.Barcode == result.Game.Barcode);

                    // Add game to confirmation list
                    if (!alreadyExists)
                    {
                        GamesToDelete.Add(result.Game);
                    }
                }
                else
                {
                    SuccessMessage = "Game not found.";
                }
            }
            catch (Exception ex)
            {
                SuccessMessage =
                    $"Error looking up barcode: {ex.Message}";
            }

            // Clear input after lookup
            Barcode = "";

            return Page();
        }

        // ==================== OPEN CONFIRMATION MODEL ====================
        // Displays the delete confirmation popup


        public IActionResult OnPostShowConfirmation()
        {

            // Only show confirmation if at least one game exists
            if (GamesToDelete.Any())
            {
                ShowConfirmation = true;
            }

            return Page();
        }

        public IActionResult OnPostCloseConfirmation()
        {
            ShowConfirmation = false;

            return Page();
        }

        // ==================== DELETE GAMES ====================
        // Deletes all selected games from database
        public async Task<IActionResult> OnPostDeleteGamesAsync()
        {
            // Prevent empty delete operations
            if (!GamesToDelete.Any())
                return Page();

            try
            {
                // ==================== BARCODE DELETE ====================
                // Build barcode list from selected games
                var barcodes = GamesToDelete
                    .Where(g => !string.IsNullOrWhiteSpace(g.Barcode))
                    .Select(g => g.Barcode!)
                    .Distinct()
                    .ToList();

                // ==================== TITLE DELETE ====================
                // Build title list from selected games
                var titles = GamesToDelete
                    .Where(g => !string.IsNullOrWhiteSpace(g.Title))
                    .Select(g => g.Title)
                    .Distinct()
                    .ToList();

                bool barcodeDeleteSuccess = true;
                bool titleDeleteSuccess = true;

                // Delete games by barcode
                if (barcodes.Any())
                {
                    barcodeDeleteSuccess = await _gameService
                        .DeleteGamesByBarcodeAsync(barcodes);
                }

                // Delete games by title
                if (titles.Any())
                {
                    titleDeleteSuccess = await _gameService
                        .DeleteGamesByTitleAsync(titles);
                }

                // Final success result
                bool success =
                    barcodeDeleteSuccess && titleDeleteSuccess;

                // Set user feedback message
                SuccessMessage = success
                    ? "Games deleted successfully!"
                    : "Failed to delete games.";

                // Clear queue after delete
                GamesToDelete.Clear();

                // Hide confirmation popup
                ShowConfirmation = false;
            }
            catch (Exception ex)
            {
                SuccessMessage =
                    $"Error deleting games: {ex.Message}";
            }

            return Page();
        }

        // ==================== LOOKUP GAME BY TITLE ====================
        // Finds a game using title search instead of barcode
        public async Task<IActionResult> OnPostLookupTitleAsync()
        {
            // Prevent empty searches
            if (string.IsNullOrWhiteSpace(Title))
                return Page();

            try
            {
                // Clean title input
                var cleanTitle = Title.Trim();

                // Call title lookup endpoint
                var game = await _gameService
                    .GetGameByTitleAsync(cleanTitle);

                // Ensure game exists
                if (game != null)
                {
                    // Prevent duplicate entries
                    bool alreadyExists = GamesToDelete.Any(g =>
                        g.Title == game.Title);

                    // Add game if not already queued
                    if (!alreadyExists)
                    {
                        GamesToDelete.Add(game);
                    }
                }
                else
                {
                    SuccessMessage = "Game not found.";
                }
            }
            catch (Exception ex)
            {
                SuccessMessage =
                    $"Error looking up title: {ex.Message}";
            }

            // Clear input
            Title = "";

            return Page();
        }

        // ==================== REMOVE GAME FROM LIST ====================
        // Removes a game from the delete queue before confirmation
        public IActionResult OnPostRemoveGame(string barcode)
        {
            // Remove matching barcode from queue
            GamesToDelete = GamesToDelete
                .Where(g => g.Barcode != barcode)
                .ToList();

            return Page();
        }
    }
}
