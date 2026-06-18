using MeepleSystemClient.Models; // Model Imports (Game, ReportGame, GetGameResponseModel, GetGameReportResponseModel, DeleteGame)
using System.Net.Http.Headers; // Lets you configure HTTP headers, like Content-Type for file uploads (used at fileContext.Headers.ContentType)
using System.Net.Http.Json; // Adds helper methods like PostAsJsonAsync() and ReadFromJsonAsync()
using System.Text.Json;


// '.Services' marks this class as part of the Services layer, which is responsible for handling business logic and communication with external APIs or databases.
namespace MeepleSystemClient.Services
{
    // The GameService Implements the IGameService interface (game object contract defined in IGameService.cs) This allows for dependency injection and easier testing.
    public class GameService : IGameService
    {
        // ==================== GAME SERVICE HELPERS ====================
        // Shared helper method for validating API responses
        private async Task ValidateResponse(
            HttpResponseMessage response,
            string endpointName)
        {
            // Check if the API response was unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                // Read the error message returned from the API
                var error = await response.Content.ReadAsStringAsync();

                // DEBUGGING: log API error details
                Console.WriteLine($"API ERROR ({endpointName})");
                Console.WriteLine(error);

                // Throw exception with endpoint name and HTTP status code
                throw new Exception(
                    $"API Error ({endpointName}): {response.StatusCode}");
            }
        }
        // HTTP COMMUNICATION OBJECT: Handles GET requests, POST requests, sending JSON and receiving JSON
        private readonly HttpClient _http;

        // CONSTRUCTOR: Injects the HttpClient into the GameService class
        public GameService(HttpClient http)
        {
            _http = http; // The _http object is responsible for sending HTTP requests and receiving HTTP responses from the API
        }

        #region GetAllGamesAsync
        public async Task<List<Game>> GetAllGamesAsync()
        {
            // Try-catch block to handle any exceptions that may occur during the API call
            try
            {
                //// Execution Tracking Debugging
                //Console.WriteLine("Calling API: GetAllGames");

                // THE API REQUESET: Sends GET request (From the Page Controller/Model/.cshtml.cs) to the API endpoint "GetAllGames" and waits for the response
                var response = await _http.GetAsync("GetAllGames");

                // Check if the response is successful (HTTP status code 200-299, 404, 500, etc.)
                if (!response.IsSuccessStatusCode)
                {
                    // If the response is not successful (log the status code and return an empty list)
                    Console.WriteLine($"GetAllGames FAILED: {response.StatusCode}");
                    // Return an empty list of games if the API call fails
                    return new List<Game>();
                }

                // Deserialize the JSON response into a DeleteGame object (which contains a list of games)
                var raw = await response.Content.ReadAsStringAsync();

                Console.WriteLine("RAW JSON:");
                Console.WriteLine(raw);

                var data = System.Text.Json.JsonSerializer.Deserialize<GetGamesResponseModel>(
                    raw,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }); ;

                //// Log the number of games returned
                //Console.WriteLine($"Games returned: {data?.gameList?.Count}");

                // Return gameList if data exist, else return an empty list
                return data?.gameList ?? new List<Game>();
            }

            // Catch any exceptions that occur during the API call and log the error message
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllGames ERROR: {ex.Message}");
                return new List<Game>();
            }
        }
        #endregion

        #region GetGameByTitleAsync
        public async Task<Game?> GetGameByTitleAsync(string title)
        {
            // Checks null empty string or whitespace, preventing bad API calls and possible errors
            if (string.IsNullOrWhiteSpace(title))
                return null;

            // Try-catch block to handle any exceptions that may occur during the API call
            try
            {
                // Clean the title by trimming whitespace and encoding it for use in a URL
                var cleanTitle = title.Trim(); // "  Catan  " -> "Catan"

                // Converts unsafe URL characters (spaces, special characters)
                var encodedTitle = Uri.EscapeDataString(cleanTitle); // "Catan: Seafarers" -> "Catan%3A%20Seafarers"

                //// DEBUGGING: console log to track API calls
                //Console.WriteLine($"Calling API: GetGameByTitle/{encodedTitle}");

                // The HTTP GET request to the GameController in the API
                var response = await _http.GetAsync($"GetGameByTitle/{encodedTitle}");

                // Check if the response is successful (HTTP status code 200-299)
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<GetGameResponseModel>(); // Desrialize JSON to Game object

                    // Null check and return the Game if it exists in the response, otherwise continue to search through all games
                    if (data?.Game != null)
                        return data.Game;
                }

                var allGames = await GetAllGamesAsync();

                // Either returns the game that matches the title (ignoring case and whitespace) or null if no match is found
                return allGames.FirstOrDefault(g =>
                    g.Title != null && // Check for null to prevent NullReferenceException
                    g.Title.Trim().Equals(cleanTitle, StringComparison.OrdinalIgnoreCase)); // "Catan" matches "  Catan  " and "catan"
            }

            // Catch any exceptions that occur during the API call and log the error message
            catch (Exception ex)
            {
                Console.WriteLine($"GetGameByTitle ERROR: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region GetGameReportAsync
        public async Task<List<ReportGame>> GetGameReportAsync(int timeframe, bool most, int quantity)
        {
            // Try-catch block to handle any exceptions that may occur during the API call
            try
            {
                // Route Construction: Builds the API route with query parameters for timeframe, most/least played, and quantity of results
                var route = $"GetGameReport/{timeframe}={most.ToString().ToLower()}={quantity}";

                //// DEBUGGING: console log to track API calls and parameters
                //Console.WriteLine($"Calling API: {route}");

                // The HTTP GET request to the GameController in the API
                var response = await _http.GetAsync(route);

                // Check if the response is successful (HTTP status code 200-299)
                if (!response.IsSuccessStatusCode)
                {
                    //// DEBUGGING: log the failure status code
                    //Console.WriteLine($"Report FAILED: {response.StatusCode}");
                    return new List<ReportGame>();
                }

                // Deserialize JSON into GetGameReportResponseModel
                var data = await response.Content.ReadFromJsonAsync<GetGameReportResponseModel>();

                // Null check and API status check
                if (data == null)
                {
                    //// DEBUGGING: log if the response was null (deserialization failure)
                    //Console.WriteLine("Report returned NULL response");
                    return new List<ReportGame>();
                }

                // API status check: Logs for failure from the API
                if (!data.Status)
                {
                    //// DEBUGGING: log the error message returned from the API
                    //Console.WriteLine($"Report API error: {data.Message}");
                    return new List<ReportGame>();
                }

                //// DEBUGGING: log the number of games returned in the report
                //Console.WriteLine($"Report games count: {data.GameList?.Count}");

                // Return the list of ReportGame objects if they exist, otherwise return an empty list
                return data.GameList ?? new List<ReportGame>();
            }
            // Catch any exceptions that occur during the API call and log the error message
            catch (Exception ex)
            {
                Console.WriteLine($"Report ERROR: {ex.Message}");
                return new List<ReportGame>();
            }
        }
        #endregion

        #region UploadCsvAsync
        public async Task<bool> UploadCsvAsync(Stream fileStream, string fileName)
        {
            // Try-catch block to handle any exceptions that may occur during the file upload process
            try
            {
                // Create a MultipartFormDataContent object to hold the file content for the HTTP POST request
                using var content = new MultipartFormDataContent();

                // Create a StreamContent object from the file stream and set the content type to "text/csv"
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv"); // This tells the API that the uploaded file is a CSV file

                // Add the file content to the multipart form data with the name "file" and the original file name
                content.Add(fileContent, "file", fileName);

                //// DEBUGGING: log the start of the upload process
                //Console.WriteLine("Uploading CSV...");

                // Send the HTTP POST request to the "InsertFromCsv" endpoint of the API with the multipart form data content
                var response = await _http.PostAsync("InsertFromCsv", content);

                //// Debugging: log the status code of the upload response
                //Console.WriteLine($"Upload result: {response.StatusCode}");

                // Return true if the upload was successful (HTTP status code 200-299), otherwise return false
                return response.IsSuccessStatusCode;
            }

            // Catch any exceptions that occur during the file upload process and log the error message
            catch (Exception ex)
            {
                Console.WriteLine($"Upload ERROR: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region ExportCsvAsync
        public async Task<byte[]?> ExportCsvAsync()
        {
            // Try-catch block to handle any exceptions that may occur during the CSV export process
            try
            {
                //// DEBUGGING: log the start of the CSV export process
                //Console.WriteLine("Downloading CSV...");

                // Send the HTTP GET request to the "ExportToCsv" endpoint of the API to download the CSV file
                var response = await _http.GetAsync("ExportToCsv");

                // Check if the response is successful (HTTP status code 200-299)
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Export FAILED: {response.StatusCode}");
                    return null;
                }

                // Read the response as a byte array (CSV content) and return it
                return await response.Content.ReadAsByteArrayAsync();
            }
            // Catch any exceptions that occur during the CSV export process and log the error message
            catch (Exception ex)
            {
                Console.WriteLine($"Export ERROR: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region CheckBarcodeAsync
        // Checks if a barcode exists in the database and returns the matching game
        public async Task<GetGameResponseModel?> CheckBarcodeAsync(string barcode)
        {
            // Prevent invalid barcode requests
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            try
            {
                // Remove leading/trailing whitespace
                var cleanBarcode = barcode.Trim();

                // Encode barcode for URL safety
                var encodedBarcode = Uri.EscapeDataString(cleanBarcode);

                // DEBUGGING: log API request
                Console.WriteLine(
                    $"Calling API: GetGameByBarcode/{encodedBarcode}");

                // IMPORTANT:
                // This endpoint returns GetGameResponseModel
                var response = await _http.GetAsync(
                    $"GetGameByBarcode/{encodedBarcode}");

                // DEBUGGING: log response status
                Console.WriteLine($"Status: {response.StatusCode}");

                // If API request failed
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Barcode lookup failed");
                    return null;
                }

                // Deserialize JSON response into GetGameResponseModel
                var result = await response.Content
                    .ReadFromJsonAsync<GetGameResponseModel>();

                // Check if deserialization failed
                if (result == null)
                {
                    Console.WriteLine("Deserialization failed");
                    return null;
                }

                // DEBUGGING: log returned game title
                Console.WriteLine($"Found: {result?.Game?.Title}");

                return result;
            }
            catch (Exception ex)
            {
                // DEBUGGING: log exception details
                Console.WriteLine("Exception in CheckBarcodeAsync:");
                Console.WriteLine(ex.Message);

                return null;
            }
        }
        #endregion

        public async Task<bool> EditGameAsync(Game game)
        {
            if (game == null)
                return false;

            try
            {
                Console.WriteLine("Calling API: EditGameByTitle");

                var request = new RegistrationModel
                {
                    Title =
                        string.IsNullOrWhiteSpace(game.Title)
                            ? ""
                            : game.Title.Trim(),

                    Barcode =
                        string.IsNullOrWhiteSpace(game.Barcode)
                            ? null
                            : game.Barcode.Trim(),

                    // REAL STYLE ID
                    StyleName = game.Style,

                    Weight = (decimal?)game.Weight,

                    LocationName =
                        string.IsNullOrWhiteSpace(game.LocationName)
                            ? null
                            : game.LocationName.Trim(),

                    SupplierName =
                        string.IsNullOrWhiteSpace(game.SupplierName)
                            ? null
                            : game.SupplierName.Trim(),

                    SellerName =
                        string.IsNullOrWhiteSpace(game.SellerName)
                            ? null
                            : game.SellerName.Trim(),

                    ImageLocation =
                        string.IsNullOrWhiteSpace(game.ImageLocation)
                            ? null
                            : game.ImageLocation.Trim(),

                    Cost = game.Cost,

                    NeedsAddedToBgg = game.NeedsAddedToBgg ?? false,

                    DateAcquired = game.DateAcquired.HasValue
                        ? DateOnly.FromDateTime(game.DateAcquired.Value)
                        : null,

                    BggGameId = game.BggGameId,

                    Expansion = game.Expansion ?? false,

                    MinPlayers = game.MinPlayers ?? 0,
                    MaxPlayers = game.MaxPlayers ?? 0,

                    RecommendedPlayers =
                        string.IsNullOrWhiteSpace(game.RecommendedPlayers)
                            ? null
                            : game.RecommendedPlayers.Trim(),

                    BestPlayers =
                        string.IsNullOrWhiteSpace(game.BestPlayers)
                            ? null
                            : game.BestPlayers.Trim(),

                    MinDuration = (short?)game.MinDuration,

                    MaxDuration = (short?)game.MaxDuration
                };

                Console.WriteLine("===== REQUEST =====");

                Console.WriteLine(
                    System.Text.Json.JsonSerializer.Serialize(request));

                var response = await _http.PostAsJsonAsync(
                    "EditGameByTitle",
                    request);

                Console.WriteLine($"Status: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(content);

                await ValidateResponse(
                    response,
                    "EditGameByTitle");

                Console.WriteLine("Game updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in EditGameAsync:");
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        // ==================== ADD GAME ====================
        // Creates a new game entry in the database
        public async Task<bool> SaveGameAsync(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            try
            {
                Console.WriteLine("Calling API: SaveGame");

                var request = new RegistrationModel
                {
                    Title =
                        string.IsNullOrWhiteSpace(game.Title)
                            ? ""
                            : game.Title.Trim(),

                    StyleName = game.Style,

                    Weight = (decimal?)game.Weight,

                    LocationName = game.LocationName,

                    SupplierName =
                        string.IsNullOrWhiteSpace(game.SupplierName)
                            ? null
                            : game.SupplierName.Trim(),

                    SellerName =
                        string.IsNullOrWhiteSpace(game.SellerName)
                            ? null
                            : game.SellerName.Trim(),

                    ImageLocation =
                        string.IsNullOrWhiteSpace(game.ImageLocation)
                            ? null
                            : game.ImageLocation.Trim(),

                    Cost = game.Cost,

                    NeedsAddedToBgg = game.NeedsAddedToBgg ?? false,

                    DateAcquired = game.DateAcquired.HasValue
                        ? DateOnly.FromDateTime(game.DateAcquired.Value)
                        : null,

                    BggGameId = game.BggGameId,

                    Expansion = game.Expansion ?? false,

                    MinDuration = (short?)game.MinDuration,

                    MaxDuration = (short?)game.MaxDuration,

                    RecommendedPlayers =
                        string.IsNullOrWhiteSpace(game.RecommendedPlayers)
                            ? null
                            : game.RecommendedPlayers.Trim(),

                    BestPlayers =
                        string.IsNullOrWhiteSpace(game.BestPlayers)
                            ? null
                            : game.BestPlayers.Trim(),
                };


                // SEND REGISTRATION MODEL
                Console.WriteLine("===== REQUEST =====");

                Console.WriteLine(
                    System.Text.Json.JsonSerializer.Serialize(request));

                var response = await _http.PostAsJsonAsync(
                    "SaveGame",
                    request);

                Console.WriteLine($"Status: {response.StatusCode}");

                await ValidateResponse(response, "SaveGame");

                Console.WriteLine("Game saved successfully");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in SaveGameAsync:");
                Console.WriteLine(ex.Message);

                return false;
            }
        }


        // ==================== CHECK IN GAME ====================
        // Marks a game as checked in by sending the title to the API
        public async Task CheckInGameAsync(string title)
        {
            try
            {
                // DEBUGGING: log API call
                Console.WriteLine("Calling API: CheckInByTitleList");

                // Create list containing game title
                var list = new List<string> { title };

                // Send POST request to API
                var response = await _http.PostAsJsonAsync(
                    "CheckInByTitleList",
                    list);

                // Validate API response using shared helper method
                await ValidateResponse(response, "CheckInByTitleList");

                // DEBUGGING: log successful check-in
                Console.WriteLine("Game checked in successfully");
            }
            catch (Exception ex)
            {
                // DEBUGGING: log exception details
                Console.WriteLine("Exception in CheckInGameAsync:");
                Console.WriteLine(ex.Message);

                throw;
            }
        }

        // ==================== ADD BARCODE ====================
        // Adds a barcode to an existing game by title
        public async Task<GameData>? AddBarcodeAsync(string title, string barcode)
        {
            try
            {
                // Remove leading/trailing whitespace
                var cleanTitle = title.Trim();

                // Remove leading/trailing whitespace
                var cleanBarcode = barcode.Trim();

                // Encode user input to prevent broken URLs and routing issues
                var encodedTitle = Uri.EscapeDataString(cleanTitle);
                var encodedBarcode = Uri.EscapeDataString(cleanBarcode);

                // DEBUGGING: log API call
                Console.WriteLine(
                    $"Calling API: AddGameBarcode/{encodedTitle}&&{encodedBarcode}");

                // Send GET request to API
                var response = await _http.GetAsync(
                    $"AddGameBarcode/{encodedTitle}&&{encodedBarcode}");

                // Validate API response using shared helper method
                await ValidateResponse(response, "AddGameBarcode");

                // DEBUGGING: log response status
                Console.WriteLine($"Status: {response.StatusCode}");

                // If API request failed
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Barcode lookup failed");
                    return null;
                }

                // Deserialize JSON response into GetGameResponseModel
                var result = await response.Content
                    .ReadFromJsonAsync<BarcodeCheckResponseModel>();

                // Check if deserialization failed
                if (result == null || result.Data == null)
                {
                    Console.WriteLine("Deserialization failed");
                    return null;
                }

                // DEBUGGING: log returned game title
                Console.WriteLine($"Found: {result?.Data?.Title}");

                return result.Data;
            }
            catch (Exception ex)
            {
                // DEBUGGING: log exception details
                Console.WriteLine("Exception in AddBarcodeAsync:");
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        // ==================== DELETE GAMES BY BARCODE ====================
        // Deletes games using a list of barcodes
        public async Task<bool> DeleteGamesByBarcodeAsync(List<string> barcodes)
        {
            // Prevent invalid delete requests
            if (barcodes == null || !barcodes.Any())
                return false;

            try
            {
                // Remove whitespace and invalid values
                var cleanedBarcodes = barcodes
                    .Where(b => !string.IsNullOrWhiteSpace(b))
                    .Select(b => b.Trim())
                    .ToList();

                // DEBUGGING: log API request
                Console.WriteLine("Calling API: DeleteGameByBarcodes");

                // Send DELETE request with JSON body
                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    "DeleteGameByBarcodes")
                {
                    Content = JsonContent.Create(cleanedBarcodes)
                };

                // Execute request
                var response = await _http.SendAsync(request);

                // DEBUGGING: log status code
                Console.WriteLine($"Status: {response.StatusCode}");

                // Validate response
                await ValidateResponse(response, "DeleteGameByBarcodes");

                Console.WriteLine("Games deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in DeleteGamesByBarcodeAsync:");
                Console.WriteLine(ex.Message);

                return false;
            }
        }
        // ==================== DELETE GAMES BY TITLE ====================
        // Deletes games using a list of titles
        public async Task<bool> DeleteGamesByTitleAsync(List<string> titles)
        {
            // Prevent invalid delete requests
            if (titles == null || !titles.Any())
                return false;

            try
            {
                // Remove whitespace and invalid values
                var cleanedTitles = titles
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .Distinct()
                    .ToList();

                // Prevent empty requests after cleaning
                if (!cleanedTitles.Any())
                    return false;

                // DEBUGGING: log API request
                Console.WriteLine("Calling API: DeleteGameByTitles");

                // Build DELETE request with JSON body
                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    "DeleteGameByTitles")
                {
                    Content = JsonContent.Create(cleanedTitles)
                };

                // Execute request
                var response = await _http.SendAsync(request);

                // DEBUGGING: log response status
                Console.WriteLine($"Status: {response.StatusCode}");

                // Validate API response
                await ValidateResponse(
                    response,
                    "DeleteGameByTitles");

                // DEBUGGING: success message
                Console.WriteLine("Games deleted successfully by title");

                return true;
            }
            catch (Exception ex)
            {
                // DEBUGGING: exception details
                Console.WriteLine(
                    "Exception in DeleteGamesByTitleAsync:");

                Console.WriteLine(ex.Message);

                return false;
            }
        }
    }
}