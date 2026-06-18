using MeepleSystemClient.Models;

// NOTES:
// This GameInterface is the promise and the GameService file is the actual implementation
// Interface (Any class that implements this must provide these methods)
// Interface allows for Easy Testing and Clean Architecture

public interface IGameService
{
    Task<List<Game>> GetAllGamesAsync();

    Task<Game?> GetGameByTitleAsync(string title);

    Task<List<ReportGame>> GetGameReportAsync(
        int timeframe,
        bool most,
        int quantity);

    Task<bool> UploadCsvAsync(
        Stream fileStream,
        string fileName);

    Task<byte[]> ExportCsvAsync();

    // Check barcode → returns game info from API
    Task<GetGameResponseModel?> CheckBarcodeAsync(
        string barcode);

    // Add barcode to an existing game
    Task<GameData>? AddBarcodeAsync(
        string title,
        string barcode);


    // ==================== CHECK IN ====================

    // Check in a game by title
    Task CheckInGameAsync(string title);

    // Edit game details
    Task<bool> EditGameAsync(Game game);

    // Save game to database
    Task<bool> SaveGameAsync(
        Game game);

    // Delete games using barcode list
    Task<bool> DeleteGamesByBarcodeAsync(
        List<string> barcodes);

    Task<bool> DeleteGamesByTitleAsync(List<string> titles);
}