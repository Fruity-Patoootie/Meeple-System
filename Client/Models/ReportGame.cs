using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

// Purpose:
// Model for the Report Results
// Used In: Report Page

public class ReportGame
{
    // The unique identifier for the game
    [JsonPropertyName("gameId")]
    public int GameId { get; set; }

    // The title of the game
    [JsonPropertyName("title")]
    public string Title { get; set; }

    // The number of times the game has been played
    [JsonPropertyName("timesPlayed")]
    public int TimesPlayed { get; set; }

    // The location where the game is played
    [JsonPropertyName("locationName")]
    public string Location { get; set; }
}