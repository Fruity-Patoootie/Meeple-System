using Azure;
using MeepleSystemAPI.Data;
using MeepleSystemAPI.IRepository;
using MeepleSystemAPI.Models;
using MeepleSystemAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;


namespace MeepleSystemAPI.Controllers
{
    public class GameController : ControllerBase
    {
        // private readonly IGame repository
        private readonly IGame repository;

        private readonly MeepleSystemContext context;

        private IConfiguration config;

        public GameController(IConfiguration Config)
        {
            config = Config;
            context = new MeepleSystemContext(config);
            repository = new GameDAL(context, config);
        }

        #region GetAllGames
        [HttpGet("GetAllGames", Name = "GetAllGames")]
        [AllowAnonymous]
        public async Task<GetGamesResponseModel> GetAllGames()
        {
            GetGamesResponseModel response = new GetGamesResponseModel();

            //set up a list to hold the incoming users we will get from the db
            List<GameData> gameList = new List<GameData>();
            try
            {
                gameList = repository.GetAllGames();

                //check the list isn't empty
                if (gameList.Count != 0)
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.gameList = gameList;
                }
                else
                {
                    //there has been an error
                    response.Status = false;
                    response.Message = "Get Failed";
                    response.StatusCode = 0;
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Get Failed";
                response.StatusCode = 0;
                //there has been an error
                Console.WriteLine(ex.Message);
            }
            return response;
        }
        #endregion

        #region GetGameByTitle
        [HttpGet("GetGameByTitle/{title}", Name = "GetGameByTitle")]
        [AllowAnonymous]
        public async Task<GetGameResponseModel> GetGameByTitle(string title)
        {
            GetGameResponseModel response = new GetGameResponseModel();

            try
            {
                if (!string.IsNullOrEmpty(title))
                {
                    // await the async repository method
                    GameData? game = await repository.FindByTitle(title);

                    if (game != null)
                    {
                        response.Status = true;
                        response.StatusCode = 200;
                        response.Game = game;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Game not found";
                        response.StatusCode = 404;
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Invalid title";
                    response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error retrieving game";
                response.StatusCode = 500;
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region GetGameByBarcode
        [HttpGet("GetGameByBarcode/{barcode}", Name = "GetGameByBarcode")]
        [AllowAnonymous]
        public async Task<GetGameResponseModel> GetGameByeBarcode(string barcode)
        {
            GetGameResponseModel response = new GetGameResponseModel();

            try
            {
                if (!string.IsNullOrEmpty(barcode))
                {
                    // await the async repository method
                    GameData game = await repository.FindByBarcode(barcode);

                    if (game != null)
                    {
                        response.Status = true;
                        response.StatusCode = 200;
                        response.Game = game;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Game not found";
                        response.StatusCode = 404;
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Invalid title";
                    response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error retrieving game";
                response.StatusCode = 500;
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region BarcodeCheck
        [HttpGet("BarcodeCheck/{barcode}", Name = "BarcodeCheck")]
        [AllowAnonymous]
        public async Task<BarcodeCheckResponseModel> BarcodeCheck(string barcode)
        {
            BarcodeCheckResponseModel res = new BarcodeCheckResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(barcode))
                {
                    // await the async repository method
                    GameData? game = await repository.BarcodeCheck(barcode);

                    if (game == null)
                    {
                        res.Status = true;
                        res.StatusCode = 404;
                        res.Message = "Barcode not assocaited with a game";
                        res.Data = game;
                    }
                    else
                    {
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Game found with barcode!";
                        res.Data = game;
                    }
                }
            }
            catch
            {
                res.Status = false;
                res.Message = "Error calling BarcodeCheck";
                res.StatusCode = 500;
                res.Data = null;
            }
            return res;
        }

        #endregion

        #region AddBarcode
        [HttpGet("AddGameBarcode/{title}&&{barcode}", Name = "AddGameBarcode")]
        [AllowAnonymous]
        public async Task<BarcodeCheckResponseModel> AddGameBarcode(string title, string barcode)
        {
            BarcodeCheckResponseModel res = new BarcodeCheckResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(title))
                {
                    // await the async repository method
                    GameData? game = await repository.AddBarcode(title, barcode);

                    if (game != null)
                    {
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Game found with barcode!";
                        res.Data = game;
                    }
                    else
                    {
                        res.Status = true;
                        res.StatusCode = 404;
                        res.Message = "Title Not found in DB, double check spelling!";
                        res.Data = game;
                    }
                }
            }
            catch
            {
                res.Status = false;
                res.Message = "Error calling AddBarcode";
                res.StatusCode = 500;
                res.Data = null;
            }
            return res;
        }

        #endregion

        #region GetGameReport
        [HttpGet("GetGameReport/{timeframe}={most}={quantity}", Name = "GetGameReport")]
        [AllowAnonymous]
        public async Task<GetReportResponseModel> GetGameReport(int timeframe, bool most, int quantity)
        {
            GetReportResponseModel response = new GetReportResponseModel();

            try
            {
                // await the async repository method
                var reportList = await repository.GetGameReport(timeframe, most, quantity).ConfigureAwait(true);

                if (reportList != null)
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.Message = "Success!";
                    response.gameList = reportList;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = 500;
                    response.Message = "List is null!";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error retrieving game";
                response.StatusCode = 500;
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region EditGame

        [HttpPost("EditGameByTitle/", Name = "EditGameByTitle")]
        [AllowAnonymous]
        public async Task<EditGameResponse> EditGameByTitle([FromBody] RegistrationModel gameModel)
        {
            EditGameResponse response = new EditGameResponse();
            try
            {
                response = await repository.EditGameRecord(gameModel);
            }
            catch
            {
                response.StatusCode = 500;
                response.Status = false;
                response.Message = "An Error occurred in API";
                response.Data = null;
            }
            return response;
        }
        #endregion

        #region SaveGame
        [HttpPost("SaveGame", Name = "SaveGame")]
        [AllowAnonymous]
        public async Task<SaveGameResponse> SaveGameRecord([FromBody] RegistrationModel gamemodel)
        {
            SaveGameResponse response = new SaveGameResponse();
            if (gamemodel.Title == null)
            {
                response.Status = false;
                response.Message = "Post failed";
                response.StatusCode = 0;
                //there has been an error

                return response;
            }
            try
            {
                response = await repository.SaveGameRecord(gamemodel);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Post failed";
                response.StatusCode = 0;
                //there has been an error
                Console.WriteLine(ex.Message);
            }
            return response;
        }
        #endregion

        #region DeleteGameByTitles
        [HttpDelete("DeleteGameByTitles", Name = "DeleteGameByTitles")]
        [AllowAnonymous]
        public async Task<List<DeleteGameResponseModel>> DeleteGameByTitles([FromBody] List<string> gameTitles)
        {
            List<DeleteGameResponseModel> response = new List<DeleteGameResponseModel>();

            try
            {
                var result = await repository.DeleteGameByTitle(gameTitles).ConfigureAwait(true);

                if (result != null)
                {
                    response = result;
                }
                else
                {
                    response.Add(new DeleteGameResponseModel
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "No games found or could not be deleted."
                    });
                }
            }
            catch (Exception ex)
            {
                response.Add(new DeleteGameResponseModel
                {
                    Status = false,
                    StatusCode = 500,
                    Message = "Error deleting games"
                });

                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region DeleteGameByBarcodes
        [HttpDelete("DeleteGameByBarcodes", Name = "DeleteGameByBarcodes")]
        [AllowAnonymous]
        public async Task<List<DeleteGameResponseModel>> DeleteGameByBarcodes([FromBody] List<string> barcodes)
        {
            List<DeleteGameResponseModel> response = new List<DeleteGameResponseModel>();

            try
            {
                var result = await repository.DeleteGameByBarcode(barcodes).ConfigureAwait(true);

                if (result != null)
                {
                    response = result;
                }
                else
                {
                    response.Add(new DeleteGameResponseModel
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "No games found or could not be deleted."
                    });
                }
            }
            catch (Exception ex)
            {
                response.Add(new DeleteGameResponseModel
                {
                    Status = false,
                    StatusCode = 500,
                    Message = "Error deleting games"
                });

                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region InsertFromCSV
        [HttpPost("InsertFromCsv", Name = "InsertFromCsv")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<InsertFromCsvResponseModel>> InsertFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new InsertFromCsvResponseModel
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "No file uploaded"
                });
            }

            try
            {
                var result = await repository.InsertFromFile(file);

                if (!result.Status)
                    return StatusCode(result.StatusCode, result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new InsertFromCsvResponseModel
                {
                    Status = false,
                    StatusCode = 500,
                    Message = ex.ToString()
                });
            }
        }
        #endregion

        #region ExportToCSV
        [HttpGet("ExportToCsv")]
        public async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var fileBytes = await repository.ExportToCsv();

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return StatusCode(500, "CSV export failed");
                }

                return File(
                    fileBytes,
                    "text/csv",
                    "games_export.csv"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region CheckInByBarcode
        [HttpPost("CheckInBarcodeList/", Name = "CheckInByBarcodeList")]
        [AllowAnonymous]
        public async Task<CheckInResponseModel> CheckInByBarcodeList(List<string> list)
        {
            CheckInResponseModel response = new CheckInResponseModel();

            try
            {
                if (list.IsNullOrEmpty())
                {
                    response.Status = false;
                    response.StatusCode = 400;
                    response.Message = "Invalid list";
                    return response;
                }

                var data = await repository.CheckInByBarcodeList(list);

                if (data == null)
                {
                    response.Status = false;
                    response.StatusCode = 500;
                    response.Message = "CheckInByBarcodeList failed";
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.Message = "Method hit successfully";
                    response.Data = data;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = 500;
                response.Message = "Error during check-in";
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion

        #region CheckInByTitle
        [HttpPost("CheckInByTitleList/", Name = "CheckInByTitleList")]
        [AllowAnonymous]
        public async Task<CheckInResponseModel> CheckInByTitleList(List<string> list)
        {
            CheckInResponseModel response = new CheckInResponseModel();

            try
            {
                if (list.IsNullOrEmpty())
                {
                    response.Status = false;
                    response.StatusCode = 400;
                    response.Message = "Invalid list";
                    return response;
                }

                var data = await repository.CheckInByTitleList(list);

                if (data == null)
                {
                    response.Status = false;
                    response.StatusCode = 500;
                    response.Message = "Error during CheckInByTitleList";
                }
                else
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.Message = "Check-in successful";
                    response.Data = data;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = 500;
                response.Message = "Error during check-in";
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion
    }

}