using MeepleSystemClient.Models;
using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeepleSystemClient.Pages
{
    [Authorize]
    public class SearchModel : PageModel
    {
        private readonly IGameService _gameService;

        public List<Game> Games { get; set; } = new();

        public string Query { get; set; } = "";

        public bool NoMatchesFound { get; set; }

        public SearchModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task OnGetAsync(string Query)
        {
            this.Query = Query ?? "";

            // Grab all games
            var allGames = await _gameService.GetAllGamesAsync();

            // Empty search -> show everything
            if (string.IsNullOrWhiteSpace(this.Query))
            {
                Games = allGames
                    .OrderBy(g => g.Title)
                    .ToList();

                return;
            }

            // Filter games
            var filteredGames = allGames
                .Where(g =>
                    !string.IsNullOrWhiteSpace(g.Title) &&
                    g.Title.Contains(
                        this.Query,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(g => g.Title)
                .ToList();

            // No matches -> show everything
            if (filteredGames.Count == 0)
            {
                NoMatchesFound = true;

                Games = allGames
                    .OrderBy(g => g.Title)
                    .ToList();

                return;
            }

            // Show filtered matches
            Games = filteredGames;
        }
    }
}