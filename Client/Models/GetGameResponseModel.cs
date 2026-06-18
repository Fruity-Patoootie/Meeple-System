using MeepleSystemClient.Models;
using System.Text.Json.Serialization;

// Purpose:
// Wraps a single game from the API response

public class GetGameResponseModel
{
    public bool Status { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    [JsonPropertyName("game")]
    public Game Game { get; set; }
}