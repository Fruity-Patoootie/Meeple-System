using MeepleSystemAPI.Models;
using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.IRepository
{
    public interface IGame
    {
        /// <summary>
        /// Returns a list of all games
        /// </summary>
        List<GameData> GetAllGames();

        /// <summary>
        /// Returns a single gameData object based on its barcode
        /// </summary>
        Task<GameData> FindByBarcode(string barcode);

        /// <summary>
        /// Returns a single game based on its title
        /// </summary>
        Task<GameData?> FindByTitle(string title);

        /// <summary>
        /// Returns a report of most/least played games for a timeframe
        /// </summary>
        Task<List<ReportGameModel>?> GetGameReport(int timeframe, bool most, int quantity);

        /// <summary>
        /// Saves a game to the database from a registration model
        /// </summary>
        Task<SaveGameResponse> SaveGameRecord(RegistrationModel gamemodel);

        /// <summary>
        /// Updates a game in the DB from a registration model
        /// </summary>
        Task<EditGameResponse> EditGameRecord(RegistrationModel gamemodel);

        /// <summary>
        /// Deletes games by title
        /// </summary>
        Task<List<DeleteGameResponseModel>> DeleteGameByTitle(List<string> gameTitles);

        /// <summary>
        /// Deletes games by barcode
        /// </summary>
        Task<List<DeleteGameResponseModel>> DeleteGameByBarcode(List<string> barcodes);

        /// <summary>
        /// Inserts games from a CSV stream
        /// </summary>
        Task<InsertFromCsvResponseModel> InsertFromCsv(Stream csvStream);

        /// <summary>
        /// Inserts games from a file (CSV or XLSX)
        /// </summary>
        Task<InsertFromCsvResponseModel> InsertFromFile(IFormFile file);

        /// <summary>
        /// Exports all games to a CSV file and returns it as a byte array
        /// </summary>
        Task<byte[]> ExportToCsv();

        /// <summary>
        /// Checks in games by barcode list
        /// </summary>
        Task<CheckInData> CheckInByBarcodeList(List<string> list);

        /// <summary>
        /// Checks in games by title list
        /// </summary>
        Task<CheckInData?> CheckInByTitleList(List<string> list);

        /// <summary>
        /// Checks if a barcode is associated with a game
        /// </summary>
        Task<GameData?> BarcodeCheck(string barcode);

        /// <summary>
        /// Adds a barcode to a game by title
        /// </summary>
        Task<GameData?> AddBarcode(string title, string barcode);
    }
}