using CsvHelper;
using CsvHelper.Configuration;
using MeepleSystemAPI.Data;
using MeepleSystemAPI.IRepository;
using MeepleSystemAPI.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;
using CsvHelper;
using MeepleSystemAPI.Data;
using MeepleSystemAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace MeepleSystemAPI.Repository
{
    public class GameDAL : IGame
    {
        private readonly MeepleSystemContext context;

        private readonly IConfiguration _config;

        public GameDAL(MeepleSystemContext Context, IConfiguration config)
        {
            context = Context;
            _config = config;
        }

        #region Get All Games Method
        /// <summary>
        /// Method that retrieves all Games in the database
        /// </summary>
        /// <returns></returns>
        public List<GameData> GetAllGames()
        {
            var gameList = new List<GameData>();

            var games = context.Games
                .Include(d => d.Seller)
                .Include(e => e.Supplier)
                .Include(f => f.Style)
                .Include(g => g.TimesPlayed)
                .Include(h => h.Location)
                .ToList();

            foreach (var game in games)
            {
                gameList.Add(new GameData
                {
                    GameId = game.GameId,
                    Title = game.Title,
                    Barcode = game.Barcode,
                    StyleName = game.Style?.StyleName ?? string.Empty,
                    Weight = game.Weight,
                    NumberOfTimesPlayed = game.TimesPlayed?.Count() ?? 0,
                    SupplierName = game.Supplier?.SupplierName ?? string.Empty,
                    SellerName = game.Seller?.SellerName ?? string.Empty,
                    LocationName = game.Location?.LocationName ?? string.Empty,
                    ImageLocation = game.ImageLocation,
                    Cost = game.Cost,
                    NeedsAddedToBgg = game.NeedsAddedToBgg,
                    DateAcquired = game.DateAcquired,
                    BggGameId = game.BggGameId,
                    Expansion = game.Expansion,
                    MinPlayers = game.MinPlayers,
                    MaxPlayers = game.MaxPlayers,
                    RecommendedPlayers = game.RecommendedPlayers,
                    BestPlayers = game.BestPlayers,
                    MinDuration = game.MinDuration,
                    MaxDuration = game.MaxDuration,
                });
            }

            return gameList;
        }

        #endregion

        #region Save Game Record Method
        /// <summary>
        /// save the game registration record into database
        /// </summary>
        /// <param name="gamemodel"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task<SaveGameResponse> SaveGameRecord(RegistrationModel gamemodel)
        {
            //set up a new object to hold our game information
            Game objApplicationGame;

            //set up a response status object to hold the response
            SaveGameResponse res = new SaveGameResponse();

            /************************** Grabbing Seller/Supplier/Location Id from registration response **************************/
            Seller? sellerQuery = null;
            Supplier? supplierQuery = null;
            Location? locationQuery = null;
            Style? styleQuery = null;

            int? sellerId = -1;
            int? supplierId = -1;
            int? locationId = -1;
            int? styleId = -1;

            #region Checking and Adding new Seller Entry to DB
            // ******************* Seller Check, if not null and in DB, set seller id equal to correct id ***********/
            if (gamemodel.SellerName != null)
            {
                sellerQuery = context.Sellers
                    .Where(x => string.Equals(x.SellerName, gamemodel.SellerName))
                    .FirstOrDefault();
                // Checking to see if provided seller is in the list
                if (sellerQuery != null)
                {
                    sellerId = sellerQuery.SellerId;
                }
            }

            // if no seller, new game has null property here
            if (gamemodel.SellerName.IsNullOrEmpty())
            {
                sellerId = null;
            }


            // ************************** Adding new Seller block *****************************
            // If we didn't find a seller one and if the seller name wasn't null
            if (sellerId == -1 && gamemodel.SellerName != null)
            {
                // Adding a new seller if they didn't have one before
                Seller newSeller = new Seller
                {
                    SellerName = gamemodel.SellerName
                };

                context.Sellers.Add(newSeller);
                await context.SaveChangesAsync();

                // Quering to grab new entry's seller id
                sellerQuery = context.Sellers
                    .Where(x => string.Equals(x.SellerName, gamemodel.SellerName)).FirstOrDefault();
                if (sellerQuery != null)
                {
                    sellerId = sellerQuery.SellerId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new seller to db while building new game obj";
                    return res;
                }
            }

            #endregion

            #region Checking and Adding new Supplier Entry to DB

            // ******************* Supplier Check, if not null and in DB, set Supplier id equal to correct id ********/
            if (gamemodel.SupplierName != null)
            {
                supplierQuery = context.Suppliers
                    .Where(x => string.Equals(x.SupplierName, gamemodel.SupplierName))
                    .FirstOrDefault();

                // Checking to see if provided seller is in the list
                if (supplierQuery != null)
                {
                    supplierId = supplierQuery.SupplierId;
                }
            }
            // if no supplier, new game has null property here
            if (gamemodel.SupplierName.IsNullOrEmpty())
            {
                supplierId = null;
            }


            // ************************** Adding new Supplier block *****************************
            // If we didn't find one, and the entry wasn't null
            if (supplierId == -1 && gamemodel.SupplierName != null)
            {
                // Adding a new supplier if they didn't have one before
                Supplier newSupplier = new Supplier
                {
                    SupplierName = gamemodel.SupplierName
                };

                // Adding the new supplier to the database
                context.Suppliers.Add(newSupplier);
                await context.SaveChangesAsync();


                // Quering to grab new entry's supplier id
                supplierQuery = context.Suppliers
                    .Where(x => string.Equals(x.SupplierName, gamemodel.SupplierName)).FirstOrDefault();
                if (supplierQuery != null)
                {
                    supplierId = supplierQuery.SupplierId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new supplier to db while building new game obj";
                    return res;
                }
            }

            #endregion

            #region Checking and Adding new Location Entry to DB
            // ******************* Location Check, if not null and in DB, set location id equal to correct id ********/
            if (gamemodel.LocationName != null)
            {
                locationQuery = context.Locations
                    .Where(x => string.Equals(x.LocationName, gamemodel.LocationName))
                    .FirstOrDefault();
                if (locationQuery != null)
                {
                    // Checking to see if provided seller is in the list
                    if (locationQuery != null)
                    {
                        locationId = locationQuery.LocationId;
                    }
                }
            }

            // if no location, new game has null property here
            if (gamemodel.LocationName.IsNullOrEmpty())
            {
                locationId = null;
            }

            // ************************** Adding new location block *****************************
            // If we didn't find one, and the entry wasn't null
            if (locationId == -1 && gamemodel.LocationName != null)
            {
                // Adding a new location if they didn't have one before
                Location newLocation = new Location
                {
                    LocationName = gamemodel.LocationName
                };

                // Adding the new location to the database

                context.Locations.Add(newLocation);
                await context.SaveChangesAsync();

                // Quering to grab new entry's location id
                locationQuery = context.Locations
                    .Where(x => string.Equals(x.LocationName, gamemodel.LocationName)).FirstOrDefault();

                if (locationQuery != null)
                {
                    locationId = locationQuery.LocationId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new location to db while building new game obj";
                    return res;
                }
            }
            #endregion

            #region Checking sytle and adding it
            // ******************* Seller Check, if not null and in DB, set seller id equal to correct id ***********/
            if (gamemodel.StyleName != null)
            {
                styleQuery = context.Styles
                    .Where(x => string.Equals(x.StyleName, gamemodel.StyleName))
                    .FirstOrDefault();
                // Checking to see if provided seller is in the list
                if (styleQuery != null)
                {
                    styleId = styleQuery.StyleId;
                }
            }

            // if no seller, new game has null property here
            if (gamemodel.StyleName.IsNullOrEmpty())
            {
                styleId = null;
            }
            #endregion

            try
            {
                //use the incoming information to populate our new game
                objApplicationGame = new Game
                {
                    Barcode = gamemodel.Barcode,
                    Title = gamemodel.Title,
                    LocationId = locationId,
                    SupplierId = supplierId,
                    SellerId = sellerId,
                    StyleId = styleId,
                    ImageLocation = gamemodel.ImageLocation,
                    Cost = gamemodel.Cost,
                    Weight = gamemodel.Weight,
                    NeedsAddedToBgg = gamemodel.NeedsAddedToBgg,
                    DateAcquired = gamemodel.DateAcquired,
                    BggGameId = gamemodel.BggGameId,
                    Expansion = gamemodel.Expansion,
                    MinPlayers = gamemodel.MinPlayers,
                    MaxPlayers = gamemodel.MaxPlayers,
                    RecommendedPlayers = gamemodel.RecommendedPlayers,
                    BestPlayers = gamemodel.BestPlayers,
                    MinDuration = gamemodel.MinDuration,
                    MaxDuration = gamemodel.MaxDuration,
                };

                //save this game in the database

                context.Games.Add(objApplicationGame);
                await context.SaveChangesAsync();


                //set the success to pass the data back
                res.StatusCode = 200;
                res.Message = "Save Successful";
                res.Status = true;
                res.GameId = objApplicationGame.GameId;
                res.GameTitle = gamemodel.Title;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveGameRecord --- " + ex.Message);
                res.Status = false;
                res.StatusCode = 0;
            }
            return res;
        }
        #endregion

        #region Delete GameByTitle Method
        /// <summary>
        /// Deletes a game from the database by its title
        /// </summary>
        /// <param name="gameTitles">A list of game titles to delete</param>
        /// <returns></returns>
        public async Task<List<DeleteGameResponseModel>> DeleteGameByTitle(List<string> gameTitles)
        {
            var responses = new List<DeleteGameResponseModel>();

            if (gameTitles == null || !gameTitles.Any())
                return responses;

            var normalizedTitles = gameTitles
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .ToList();

            var games = await context.Games
                .Where(g => normalizedTitles.Contains(g.Title.ToLower()))
                .ToListAsync();

            var foundTitles = games.Select(g => g.Title.ToLower()).ToHashSet();

            foreach (var title in normalizedTitles)
            {
                if (!foundTitles.Contains(title))
                {
                    responses.Add(new DeleteGameResponseModel
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = $"Game '{title}' not found."
                    });
                }
            }

            if (!games.Any())
                return responses;

            var sellerIds = games.Where(g => g.SellerId != null).Select(g => g.SellerId).Distinct().ToList();
            var supplierIds = games.Where(g => g.SupplierId != null).Select(g => g.SupplierId).Distinct().ToList();
            var locationIds = games.Where(g => g.LocationId != null).Select(g => g.LocationId).Distinct().ToList();

            context.Games.RemoveRange(games);

            foreach (var id in sellerIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.SellerId == id);

                if (!stillUsed)
                {
                    var seller = await context.Sellers.FirstOrDefaultAsync(s => s.SellerId == id);
                    if (seller != null)
                        context.Sellers.Remove(seller);
                }
            }

            foreach (var id in supplierIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.SupplierId == id);

                if (!stillUsed)
                {
                    var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
                    if (supplier != null)
                        context.Suppliers.Remove(supplier);
                }
            }

            foreach (var id in locationIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.LocationId == id);

                if (!stillUsed)
                {
                    var location = await context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);
                    if (location != null)
                        context.Locations.Remove(location);
                }
            }

            var rows = await context.SaveChangesAsync();
            Console.WriteLine($"Rows affected: {rows}");

            foreach (var game in games)
            {
                responses.Add(new DeleteGameResponseModel
                {
                    Status = true,
                    StatusCode = 200,
                    Message = $"Deleted '{game.Title}' successfully.",
                    GameTitle = game.Title,
                    SellerDeleted = sellerIds.Contains(game.SellerId),
                    SupplierDeleted = supplierIds.Contains(game.SupplierId),
                    LocationDeleted = locationIds.Contains(game.LocationId)
                });
            }

            return responses;
        }
        #endregion

        #region EditGameRecord Method
        public async Task<EditGameResponse> EditGameRecord(RegistrationModel gamemodel)
        {
            EditGameResponse res = new EditGameResponse();

            /************************** Grabbing Seller/Supplier/Location Id from registration response **************************/
            Seller? sellerQuery = null;
            Supplier? supplierQuery = null;
            Location? locationQuery = null;

            int? sellerId = -1;
            int? supplierId = -1;
            int? locationId = -1;
            int? styleId = -1;

            #region Checking and Adding new Seller Entry to DB
            // ******************* Seller Check, if not null and in DB, set seller id equal to correct id ***********/
            if (gamemodel.SellerName != null)
            {
                sellerQuery = context.Sellers
                    .Where(x => string.Equals(x.SellerName, gamemodel.SellerName))
                    .FirstOrDefault();
                // Checking to see if provided seller is in the list
                if (sellerQuery != null)
                {
                    sellerId = sellerQuery.SellerId;
                }
            }

            // if no seller, new game has null property here
            if (gamemodel.SellerName.IsNullOrEmpty())
            {
                sellerId = null;
            }


            // ************************** Adding new Seller block *****************************
            // If we didn't find a seller one and if the seller name wasn't null
            if (sellerId == -1 && gamemodel.SellerName != null)
            {
                // Adding a new seller if they didn't have one before
                Seller newSeller = new Seller
                {
                    SellerName = gamemodel.SellerName
                };

                context.Sellers.Add(newSeller);
                await context.SaveChangesAsync();

                // Quering to grab new entry's seller id
                sellerQuery = context.Sellers
                    .Where(x => string.Equals(x.SellerName, gamemodel.SellerName)).FirstOrDefault();
                if (sellerQuery != null)
                {
                    sellerId = sellerQuery.SellerId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new seller to db while building new game obj";
                    return res;
                }
            }

            #endregion

            #region Checking and Adding new Supplier Entry to DB

            // ******************* Supplier Check, if not null and in DB, set Supplier id equal to correct id ********/
            if (gamemodel.SupplierName != null)
            {
                supplierQuery = context.Suppliers
                    .Where(x => string.Equals(x.SupplierName, gamemodel.SupplierName))
                    .FirstOrDefault();

                // Checking to see if provided seller is in the list
                if (supplierQuery != null)
                {
                    supplierId = supplierQuery.SupplierId;
                }
            }
            // if no supplier, new game has null property here
            if (gamemodel.SupplierName.IsNullOrEmpty())
            {
                supplierId = null;
            }


            // ************************** Adding new Supplier block *****************************
            // If we didn't find one, and the entry wasn't null
            if (supplierId == -1 && gamemodel.SupplierName != null)
            {
                // Adding a new supplier if they didn't have one before
                Supplier newSupplier = new Supplier
                {
                    SupplierName = gamemodel.SupplierName
                };

                // Adding the new supplier to the database
                context.Suppliers.Add(newSupplier);
                await context.SaveChangesAsync();


                // Quering to grab new entry's supplier id
                supplierQuery = context.Suppliers
                    .Where(x => string.Equals(x.SupplierName, gamemodel.SupplierName)).FirstOrDefault();
                if (supplierQuery != null)
                {
                    supplierId = supplierQuery.SupplierId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new supplier to db while building new game obj";
                    return res;
                }
            }

            #endregion

            #region Checking and Adding new Location Entry to DB
            // ******************* Location Check, if not null and in DB, set location id equal to correct id ********/
            if (gamemodel.LocationName != null)
            {
                locationQuery = context.Locations
                    .Where(x => string.Equals(x.LocationName, gamemodel.LocationName))
                    .FirstOrDefault();

                // Checking to see if provided seller is in the list
                if (locationQuery != null)
                {
                    locationId = locationQuery.LocationId;
                }
            }
            // if no seller, new game has null property here
            if (gamemodel.LocationName.IsNullOrEmpty())
            {
                locationId = null;
            }

            // ************************** Adding new location block *****************************
            // If we didn't find one, and the entry wasn't null
            if (locationId == -1 && gamemodel.LocationName != null)
            {
                // Adding a new location if they didn't have one before
                Location newLocation = new Location
                {
                    LocationName = gamemodel.LocationName
                };

                // Adding the new location to the database

                context.Locations.Add(newLocation);
                await context.SaveChangesAsync();

                // Quering to grab new entry's location id
                locationQuery = context.Locations
                    .Where(x => string.Equals(x.LocationName, gamemodel.LocationName)).FirstOrDefault();

                if (locationQuery != null)
                {
                    locationId = locationQuery.LocationId;
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 0;
                    res.Message = "Failed to add new location to db while building new game obj";
                    return res;
                }
            }
            #endregion

            #region Checking sytle and adding it
            // ******************* Seller Check, if not null and in DB, set seller id equal to correct id ***********/
            if (gamemodel.StyleName != null)
            {
                var styleQuery = context.Styles
                    .Where(x => string.Equals(x.StyleName, gamemodel.StyleName))
                    .FirstOrDefault();
                // Checking to see if provided seller is in the list
                if (styleQuery != null)
                {
                    styleId = styleQuery.StyleId;
                }
            }

            // if no seller, new game has null property here
            if (gamemodel.StyleName.IsNullOrEmpty())
            {
                styleId = null;
            }
            #endregion

            try
            {
                var gameQuery = context.Games
                    .Where(x => x.Title.ToLower() == gamemodel.Title.ToLower())
                    .FirstOrDefault();
                if (gameQuery != null)
                {
                    // Set the query to the new model
                    gameQuery.Barcode = gamemodel.Barcode;
                    gameQuery.Title = gamemodel.Title;
                    gameQuery.LocationId = locationId;
                    gameQuery.SupplierId = supplierId;
                    gameQuery.SellerId = sellerId;
                    gameQuery.StyleId = styleId;
                    gameQuery.ImageLocation = gamemodel.ImageLocation;
                    gameQuery.Cost = gamemodel.Cost;
                    gameQuery.Weight = gamemodel.Weight;
                    gameQuery.NeedsAddedToBgg = gamemodel.NeedsAddedToBgg;
                    gameQuery.DateAcquired = gamemodel.DateAcquired;
                    gameQuery.BggGameId = gamemodel.BggGameId;
                    gameQuery.Expansion = gamemodel.Expansion;
                    gameQuery.MinPlayers = gamemodel.MinPlayers;
                    gameQuery.MaxPlayers = gamemodel.MaxPlayers;
                    gameQuery.RecommendedPlayers = gamemodel.RecommendedPlayers;
                    gameQuery.BestPlayers = gamemodel.BestPlayers;
                    gameQuery.MinDuration = gamemodel.MinDuration;
                    gameQuery.MaxDuration = gamemodel.MaxDuration;


                    await context.SaveChangesAsync();

                    var game = await FindByTitle(gameQuery.Title);

                    res.Message = "Successfully updated game details";
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Data = game;
                    return res;
                }
            }
            catch
            {
                res.Message = "Failed to update game details";
                res.Status = false;
                res.StatusCode = 500;
                res.Data = null;

                return res;
            }
            res.Message = "Failed to update game details";
            res.Status = false;
            res.StatusCode = 500;
            res.Data = null;
            return res;
        }

        #endregion

        #region Delete GameByBarcode Method
        /// <summary>
        /// Deletes a game from the database
        /// </summary>
        /// <param name="barcodes">A list of game barcodes to delete</param>
        /// <returns></returns>
        public async Task<List<DeleteGameResponseModel>> DeleteGameByBarcode(List<string> barcodes)
        {
            var responses = new List<DeleteGameResponseModel>();

            if (barcodes == null || !barcodes.Any())
                return responses;

            var normalizedBarcodes = barcodes
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b.Trim().ToLower())
                .ToList();

            var games = await context.Games
                .Where(g => normalizedBarcodes.Contains(g.Barcode.ToLower()))
                .ToListAsync();

            var foundBarcodes = games.Select(g => g.Barcode.ToLower()).ToHashSet();

            foreach (var barcode in normalizedBarcodes)
            {
                if (!foundBarcodes.Contains(barcode))
                {
                    responses.Add(new DeleteGameResponseModel
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = $"Game '{barcode}' not found."
                    });
                }
            }

            if (!games.Any())
                return responses;

            var sellerIds = games.Where(g => g.SellerId != null).Select(g => g.SellerId).Distinct().ToList();
            var supplierIds = games.Where(g => g.SupplierId != null).Select(g => g.SupplierId).Distinct().ToList();
            var locationIds = games.Where(g => g.LocationId != null).Select(g => g.LocationId).Distinct().ToList();

            context.Games.RemoveRange(games);

            foreach (var id in sellerIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.SellerId == id);

                if (!stillUsed)
                {
                    var seller = await context.Sellers.FirstOrDefaultAsync(s => s.SellerId == id);
                    if (seller != null)
                        context.Sellers.Remove(seller);
                }
            }

            foreach (var id in supplierIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.SupplierId == id);

                if (!stillUsed)
                {
                    var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
                    if (supplier != null)
                        context.Suppliers.Remove(supplier);
                }
            }

            foreach (var id in locationIds)
            {
                bool stillUsed = await context.Games.AnyAsync(g => g.LocationId == id);

                if (!stillUsed)
                {
                    var location = await context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);
                    if (location != null)
                        context.Locations.Remove(location);
                }
            }

            var rows = await context.SaveChangesAsync();
            Console.WriteLine($"Rows affected: {rows}");

            foreach (var game in games)
            {
                responses.Add(new DeleteGameResponseModel
                {
                    Status = true,
                    StatusCode = 200,
                    Message = $"Deleted '{game.Title}' successfully.",
                    GameTitle = game.Title,
                    SellerDeleted = sellerIds.Contains(game.SellerId),
                    SupplierDeleted = supplierIds.Contains(game.SupplierId),
                    LocationDeleted = locationIds.Contains(game.LocationId)
                });
            }

            return responses;
        }
        #endregion

        #region Find Game By Barcode Method
        public async Task<GameData> FindByBarcode(string barcode)
        {
            //create a variable to hold the game data result
            GameData? game = null;

            try
            {
                //make sure a game name was provided
                if (barcode != null)
                {
                    //query the database for a game matching the given name
                    //also include TimesPlayed so we can count related records
                    var gameQuery = context.Games
                        .Include(g => g.TimesPlayed)
                        .Include(b => b.Supplier)
                        .Include(c => c.Seller)
                        .Include(g => g.Style)
                        .Include(e => e.Location)
                        .Where(x => x.Barcode == barcode)
                        .FirstOrDefault();



                    // ***************** Location grabbing queries *************************/
                    // Making location usable in future checks
                    Location? location = null;

                    try
                    {
                        // Query the location table
                        location = context.Locations
                            .Where(d => d.LocationId == gameQuery.LocationId)
                            .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetGameReport --- " + ex.Message);
                    }

                    //if a matching game was found
                    if (gameQuery != null)
                    {
                        //map database entity to GameData object
                        game = new GameData
                        {
                            GameId = gameQuery.GameId,
                            Title = gameQuery.Title,
                            Barcode = gameQuery.Barcode,
                            StyleName = gameQuery.Style?.StyleName ?? null,
                            Weight = gameQuery.Weight,
                            NumberOfTimesPlayed = gameQuery.TimesPlayed != null ? gameQuery.TimesPlayed.Count : 0,
                            SupplierName = gameQuery.Supplier?.SupplierName ?? null,
                            SellerName = gameQuery.Seller?.SellerName ?? null,
                            LocationName = gameQuery.Location?.LocationName ?? "Unknown Location",
                            ImageLocation = gameQuery.ImageLocation,
                            Cost = gameQuery.Cost,
                            NeedsAddedToBgg = gameQuery.NeedsAddedToBgg,
                            DateAcquired = gameQuery.DateAcquired,
                            BggGameId = gameQuery.BggGameId,
                            Expansion = gameQuery.Expansion,
                            MinPlayers = gameQuery.MinPlayers,
                            MaxPlayers = gameQuery.MaxPlayers,
                            RecommendedPlayers = gameQuery.RecommendedPlayers,
                            BestPlayers = gameQuery.BestPlayers,
                            MinDuration = gameQuery.MinDuration,
                            MaxDuration = gameQuery.MaxDuration,
                        };
                    }
                }

                //return the found game (or null if not found)
                return game;
            }
            catch (Exception ex)
            {
                //log the error for debugging
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            //return whatever result we have (likely null if error occurred)
            return game;
        }

        #endregion

        #region BarcodeCheck Method
        public async Task<GameData?> BarcodeCheck(string barcode)
        {
            //create a variable to hold the game data result
            GameData? game = null;

            try
            {
                //make sure a game name was provided
                if (barcode != null)
                {
                    //query the database for a game matching the given barcode
                    //also include TimesPlayed so we can count related records
                    var gameQuery = context.Games
                        .Include(g => g.TimesPlayed)
                        .Include(b => b.Supplier)
                        .Include(c => c.Seller)
                        .Include(g => g.Style)
                        .Include(e => e.Location)
                        .Where(x => x.Barcode == barcode)
                        .FirstOrDefault();

                    if (gameQuery == null)
                    {
                        game = null;
                    }
                    else
                    {
                        game = new GameData
                        {
                            GameId = gameQuery.GameId,
                            Title = gameQuery.Title,
                            Barcode = gameQuery.Barcode,
                            StyleName = gameQuery.Style?.StyleName ?? " ",
                            Weight = gameQuery.Weight,
                            NumberOfTimesPlayed = gameQuery.TimesPlayed != null ? gameQuery.TimesPlayed.Count : 0,
                            SupplierName = gameQuery.Supplier?.SupplierName ?? " ",
                            SellerName = gameQuery.Seller?.SellerName ?? " ",
                            LocationName = gameQuery.Location?.LocationName ?? "Unknown Location",
                            ImageLocation = gameQuery.ImageLocation ?? " ",
                            Cost = gameQuery.Cost,
                            NeedsAddedToBgg = gameQuery.NeedsAddedToBgg,
                            DateAcquired = gameQuery.DateAcquired,
                            BggGameId = gameQuery.BggGameId,
                            Expansion = gameQuery.Expansion,
                            MinPlayers = gameQuery.MinPlayers,
                            MaxPlayers = gameQuery.MaxPlayers,
                            RecommendedPlayers = gameQuery.RecommendedPlayers,
                            BestPlayers = gameQuery.BestPlayers,
                            MinDuration = gameQuery.MinDuration,
                            MaxDuration = gameQuery.MaxDuration,
                        };
                    }
                    
                }

                //return the found game (or null if not found)
                return game;
            }
            catch (Exception ex)
            {
                //log the error for debugging
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            //return whatever result we have (likely null if error occurred)
            return game;
        }

        #endregion

        #region AddBarcode Method
        public async Task<GameData?> AddBarcode(string title, string barcode)
        {
            //create a variable to hold the game data result
            GameData? game = null;


            try
            {
                //make sure a game name was provided
                if (barcode != null && title != null)
                {
                    //query the database for a game matching the given title
                    var gameQuery = context.Games
                        .Include(g => g.TimesPlayed)
                        .Include(b => b.Supplier)
                        .Include(c => c.Seller)
                        .Include(g => g.Style)
                        .Include(e => e.Location)
                        .Where(x => x.Title.ToLower() == title.ToLower())
                        .FirstOrDefault();

                    // if we don't get a query, return null
                    if (gameQuery == null)
                    {
                        game = null;
                    }

                    // otherwise we hit the title! woo! set the barcode and save the db
                    else
                    {
                        // Set the game's barcode to barcode and save.
                        gameQuery.Barcode = barcode;
                        await context.SaveChangesAsync();

                        // Set the gameData return obj to the queried game
                        game = new GameData
                        {
                            GameId = gameQuery.GameId,
                            Title = gameQuery.Title,
                            Barcode = gameQuery.Barcode,
                            StyleName = gameQuery.Style?.StyleName ?? " ",
                            Weight = gameQuery.Weight,
                            NumberOfTimesPlayed = gameQuery.TimesPlayed != null ? gameQuery.TimesPlayed.Count : 0,
                            SupplierName = gameQuery.Supplier?.SupplierName ?? " ",
                            SellerName = gameQuery.Seller?.SellerName ?? " ",
                            LocationName = gameQuery.Location?.LocationName ?? "Unknown Location",
                            ImageLocation = gameQuery.ImageLocation ?? " ",
                            Cost = gameQuery.Cost,
                            NeedsAddedToBgg = gameQuery.NeedsAddedToBgg,
                            DateAcquired = gameQuery.DateAcquired,
                            BggGameId = gameQuery.BggGameId,
                            Expansion = gameQuery.Expansion,
                            MinPlayers = gameQuery.MinPlayers,
                            MaxPlayers = gameQuery.MaxPlayers,
                            RecommendedPlayers = gameQuery.RecommendedPlayers,
                            BestPlayers = gameQuery.BestPlayers,
                            MinDuration = gameQuery.MinDuration,
                            MaxDuration = gameQuery.MaxDuration,
                        };
                    }

                }

                //return the found game (or null if not found)
                return game;
            }
            catch (Exception ex)
            {
                //log the error for debugging
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            //return whatever result we have (likely null if error occurred)
            return game;
        }
        #endregion

        #region Find Game By Title Method
        public async Task<GameData?> FindByTitle(string gamename)
        {
            //create a variable to hold the game data result
            GameData? game = null;

            try
            {
                //make sure a game name was provided
                if (gamename != null)
                {
                    //query the database for a game matching the given name
                    //also include TimesPlayed so we can count related records
                    var gameQuery = context.Games
                        .Include(g => g.TimesPlayed)
                        .Include(b => b.Supplier)
                        .Include(c => c.Seller)
                        .Include(g => g.Style)
                        .Include(e => e.Location)
                        .Where(x => x.Title.ToLower() == gamename.ToLower())
                        .FirstOrDefault();



                    // ***************** Location grabbing queries *************************/
                    // Making location usable in future checks
                    Location? location = null;

                    try
                    {
                        // Query the location table
                        location = context.Locations
                            .Where(d => d.LocationId == gameQuery.LocationId)
                            .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetGameReport --- " + ex.Message);
                    }

                    //if a matching game was found
                    if (gameQuery != null)
                    {
                        //map database entity to GameData object
                        game = new GameData
                        {
                            GameId = gameQuery.GameId,
                            Title = gameQuery.Title,
                            Barcode = gameQuery.Barcode,
                            StyleName = gameQuery.Style?.StyleName,
                            Weight = gameQuery.Weight,
                            NumberOfTimesPlayed = gameQuery.TimesPlayed != null ? gameQuery.TimesPlayed.Count : 0,
                            SupplierName = gameQuery.Supplier?.SupplierName,
                            SellerName = gameQuery.Seller?.SellerName,
                            LocationName = gameQuery.Location?.LocationName ?? "Unknown Location",
                            ImageLocation = gameQuery.ImageLocation,
                            Cost = gameQuery.Cost,
                            NeedsAddedToBgg = gameQuery.NeedsAddedToBgg,
                            DateAcquired = gameQuery.DateAcquired,
                            BggGameId = gameQuery.BggGameId,
                            Expansion = gameQuery.Expansion,
                            MinPlayers = gameQuery.MinPlayers,
                            MaxPlayers = gameQuery.MaxPlayers,
                            RecommendedPlayers = gameQuery.RecommendedPlayers,
                            BestPlayers = gameQuery.BestPlayers,
                            MinDuration = gameQuery.MinDuration,
                            MaxDuration = gameQuery.MaxDuration,
                        };
                    }
                }

                //return the found game (or null if not found)
                return game;
            }
            catch (Exception ex)
            {
                //log the error for debugging
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            //return whatever result we have (likely null if error occurred)
            return game;
        }

        #endregion

        #region GetGameReport
        /// <summary>
        /// Method that retrieves data from the database and builds a report from it. Accepts an int input for the time the report is requested, a boolean for most or least played, and the quantity of games they would like on the report.
        /// Int input for the time selection is 0- Weekly; 1- Monthly, 2-Yearly, 3-All time.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ReportGameModel>?> GetGameReport(int timeframe, bool most, int quantity)
        {
            // Error handling try block for all queries
            try
            {
                // Getting the current date in UTC time. This will be our frame of reference for the query.
                DateTime currentDate = DateTime.UtcNow;

                // Grabbing all the games in the database
                List<Game> allGames = context.Games
                    .Include(d => d.Location)
                    .ToList();

                //*************************** Time Played Query Section *************************/
                DateTime timeAgo = new DateTime();
                if (timeframe == 0)
                {
                    // timeAgo is one week from the current day
                    timeAgo = currentDate.AddDays(-7);
                }
                else if (timeframe == 1)
                {
                    // timeAgo is now one month ago
                    timeAgo = currentDate.AddMonths(-1);
                }
                else if (timeframe == 2)
                {
                    // Subtracting a year from the current date
                    timeAgo = currentDate.AddYears(-1);
                }
                else if (timeframe == 3)
                {
                    // longAgo is an arbitrary DateTimeObject set in the past
                    timeAgo = new DateTime(1969, 6, 28, 1, 0, 0);
                }


                // Report game model that will be returned from the function
                List<ReportGameModel> reportList = new List<ReportGameModel>();

                // For each game, counting the times played
                foreach (Game game in allGames)
                {
                    // Converting the timeAgo obj to DateOnly to match the db format
                    DateOnly dateOnlyTimeAgo = DateOnly.FromDateTime(timeAgo);

                    // Get the count of TimePlayed Objects from the past week (aka the amount of times played)
                    int timesPlayedCount = context.TimesPlayed
                        .Where(d => d.Time > dateOnlyTimeAgo && d.GameId == game.GameId)
                        .Count();


                    // Make a new report game model
                    ReportGameModel reportGame = new ReportGameModel
                    {
                        Title = game.Title,
                        GameId = game.GameId,
                        TimesPlayed = timesPlayedCount,
                        Location = game.Location?.LocationName ?? "Unknown",
                    };

                    // Add it to the list
                    reportList.Add(reportGame);
                }

                // ************************* Sort section ***************************/
                List<ReportGameModel> returnList = new List<ReportGameModel>();
                if (most)
                {
                    // If they wanted to see the most, we order by descending so the highest value is at index[0]
                    reportList = reportList.OrderByDescending(p => p.TimesPlayed).ToList();

                }
                else
                {
                    // If they want to see least, we do a normal order by
                    reportList = reportList.OrderBy(p => p.TimesPlayed).ToList();
                }


                // ************************* Quantity section ***************************/

                // Error handling in case they request more games than in the database
                if (quantity > allGames.Count())
                {
                    quantity = allGames.Count();
                }

                // for the quantity wanted, add it to the list
                for (int i = 0; i < quantity; i++)
                {
                    returnList.Add(reportList[i]);
                }

                // Return the list
                return returnList;
            }
            catch (Exception ex)
            {
                //log the error for debugging
                Console.WriteLine("GetGameReport --- " + ex.Message);
                return null;
            }

        }
        #endregion

        #region InsertFromFile

        public async Task<InsertFromCsvResponseModel> InsertFromFile(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            Stream stream;

            if (extension == ".xlsx")
            {
                stream = ConvertExcelToCsv(file.OpenReadStream());
            }
            else if (extension == ".csv")
            {
                stream = file.OpenReadStream();
            }
            else
            {
                return new InsertFromCsvResponseModel
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Only .csv and .xlsx files are supported"
                };
            }

            return await InsertFromCsv(stream);
        }

        #endregion

        #region ConvertExcelToCsv

        private MemoryStream ConvertExcelToCsv(Stream excelStream)
        {
            var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var lastColumn = worksheet.FirstRowUsed().LastCellUsed().Address.ColumnNumber;

            foreach (var row in worksheet.RowsUsed())
            {
                for (int i = 1; i <= lastColumn; i++)
                {
                    csv.WriteField(row.Cell(i).GetValue<string>());
                }

                csv.NextRecord();
            }

            writer.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        #endregion

        #region InsertFromCsv

        public async Task<InsertFromCsvResponseModel> InsertFromCsv(Stream csvStream)
        {
            var res = new InsertFromCsvResponseModel();

            try
            {
                csvStream.Position = 0;

                using var reader = new StreamReader(csvStream);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    IgnoreBlankLines = true
                };

                using var csv = new CsvReader(reader, config);

                // IMPORTANT: materialize immediately (removes lazy streaming issues)
                var records = csv.GetRecords<GameCsvModel>().ToList();

                foreach (var record in records)
                {
                    try
                    {
                        var registration = MapCsvToRegistration(record);

                        NormalizeRegistrationModel(registration);

                        var response = await SaveGameRecord(registration);

                        if (response == null || response.Status == false)
                        {
                            res.FailedCount++;
                        }
                        else
                        {
                            res.InsertedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        res.FailedCount++;
                        // optionally log ex
                    }
                }

                res.Status = true;
                res.StatusCode = 200;
                res.Message = $"Inserted {res.InsertedCount} games. Failed: {res.FailedCount}";

                return res;
            }
            catch (Exception ex)
            {
                return new InsertFromCsvResponseModel
                {
                    Status = false,
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region NormalizeRegistrationModel

        private void NormalizeRegistrationModel(RegistrationModel m)
        {
            // =========================
            // Shared validation helper
            // =========================
            static bool IsInvalid(string? v)
                => string.IsNullOrWhiteSpace(v)
                   || v.Trim().Equals("#N/A", StringComparison.OrdinalIgnoreCase)
                   || v.Trim() == "-";

            // =========================
            // String cleanup
            // =========================
            static string? CleanString(string? v)
                => IsInvalid(v) ? null : v!.Trim();

            // =========================
            // Boolean cleanup
            // =========================
            static bool? CleanBool(object? v)
            {
                if (v == null) return null;

                var s = v.ToString()?.Trim().ToLowerInvariant();
                if (IsInvalid(s)) return null;

                return s switch
                {
                    "true" or "1" or "yes" or "y" => true,
                    "false" or "0" or "no" or "n" => false,
                    _ => null
                };
            }

            // =========================
            // Integer cleanup
            // =========================
            static int? CleanInt(object? v)
            {
                if (v == null) return null;

                var s = v.ToString()?.Trim();
                if (IsInvalid(s)) return null;

                return int.TryParse(s, out var result) ? result : null;
            }

            // =========================
            // Short cleanup
            // =========================
            static short? CleanShort(object? v)
            {
                if (v == null) return null;

                var s = v.ToString()?.Trim();
                if (IsInvalid(s)) return null;

                return short.TryParse(s, out var result) ? result : null;
            }

            // =========================
            // Decimal cleanup
            // =========================
            static decimal? CleanDecimal(object? v)
            {
                if (v == null) return null;

                var s = v.ToString()?.Trim();
                if (IsInvalid(s)) return null;

                return decimal.TryParse(s, out var result) ? result : null;
            }

            // =========================
            // Date cleanup
            // =========================
            static DateOnly? CleanDate(object? v)
            {
                if (v == null) return null;

                var s = v.ToString()?.Trim();
                if (IsInvalid(s)) return null;

                return DateOnly.TryParse(s, out var result) ? result : null;
            }

            // =========================
            // STRING FIELDS
            // =========================
            m.Title = CleanString(m.Title) ?? m.Title; // Title is required, keep fallback
            m.Barcode = CleanString(m.Barcode);
            m.StyleName = CleanString(m.StyleName);
            m.LocationName = CleanString(m.LocationName);
            m.SupplierName = CleanString(m.SupplierName);
            m.SellerName = CleanString(m.SellerName);
            m.ImageLocation = CleanString(m.ImageLocation);
            m.RecommendedPlayers = CleanString(m.RecommendedPlayers);
            m.BestPlayers = CleanString(m.BestPlayers);

            // =========================
            // NUMERIC / BOOL FIELDS
            // =========================
            m.Cost = CleanDecimal(m.Cost);
            m.Weight = CleanDecimal(m.Weight);

            m.NeedsAddedToBgg = CleanBool(m.NeedsAddedToBgg);
            m.Expansion = CleanBool(m.Expansion);

            m.BggGameId = CleanInt(m.BggGameId);

            m.MinPlayers = CleanInt(m.MinPlayers);
            m.MaxPlayers = CleanInt(m.MaxPlayers);

            m.MinDuration = CleanShort(m.MinDuration);
            m.MaxDuration = CleanShort(m.MaxDuration);

            m.DateAcquired = CleanDate(m.DateAcquired);
        }

        #endregion

        #region MapCsvToRegistration

        private RegistrationModel MapCsvToRegistration(GameCsvModel record)
        {
            static bool? ParseBool(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                return value.ToLower() switch
                {
                    "true" or "1" or "yes" or "y" => true,
                    "false" or "0" or "no" or "n" => false,
                    _ => null
                };
            }

            static decimal? ParseDecimal(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                return decimal.TryParse(value, out var result)
                    ? result
                    : null;
            }

            static int? ParseInt(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                return int.TryParse(value, out var result)
                    ? result
                    : null;
            }

            static short? ParseShort(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                return short.TryParse(value, out var result)
                    ? result
                    : null;
            }

            static DateOnly? ParseDate(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                // Handles:
                // 8/1/2025 12:00:00 AM
                // 8/1/2025
                if (DateTime.TryParse(value, out var dt))
                    return DateOnly.FromDateTime(dt);

                // fallback
                if (DateOnly.TryParse(value, out var d))
                    return d;

                return null;
            }

            static string? Clean(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                value = value.Trim();

                if (value == "-" ||
                    value.Equals("#N/A", StringComparison.OrdinalIgnoreCase))
                    return null;

                return value;
            }

            // =========================
            // DEBUGGING OUTPUT
            // =========================
            Console.WriteLine($@"
    =========================
    CSV RECORD
    =========================
    Title: {record.Title}
    Barcode: {record.Barcode}
    Location: {record.LocationName}
    Supplier: {record.SupplierName}
    Seller: {record.SellerName}
    Style: {record.StyleName}
    ImageLocation: {record.ImageLocation}
    Cost: {record.Cost}
    Weight: {record.Weight}
    NeedsAddedToBgg: {record.NeedsAddedToBgg}
    DateAcquired: {record.DateAcquired}
    BggGameId: {record.BggGameId}
    Expansion: {record.Expansion}
    MinPlayers: {record.MinPlayers}
    MaxPlayers: {record.MaxPlayers}
    RecommendedPlayers: {record.RecommendedPlayers}
    BestPlayers: {record.BestPlayers}
    MinDuration: {record.MinDuration}
    MaxDuration: {record.MaxDuration}
    =========================
    ");

            return new RegistrationModel
            {
                // =========================
                // REQUIRED
                // =========================
                Title = Clean(record.Title) ?? "UNKNOWN TITLE",

                // =========================
                // STRINGS
                // =========================
                Barcode = Clean(record.Barcode),

                StyleName = Clean(record.StyleName),
                LocationName = Clean(record.LocationName),
                SupplierName = Clean(record.SupplierName),
                SellerName = Clean(record.SellerName),

                ImageLocation = Clean(record.ImageLocation),

                RecommendedPlayers = Clean(record.RecommendedPlayers),
                BestPlayers = Clean(record.BestPlayers),

                // =========================
                // DECIMALS
                // =========================
                Cost = ParseDecimal(record.Cost),
                Weight = ParseDecimal(record.Weight),

                // =========================
                // BOOLS
                // =========================
                NeedsAddedToBgg = ParseBool(record.NeedsAddedToBgg),
                Expansion = ParseBool(record.Expansion),

                // =========================
                // DATE
                // =========================
                DateAcquired = ParseDate(record.DateAcquired),

                // =========================
                // INTS
                // =========================
                BggGameId = ParseInt(record.BggGameId),

                MinPlayers = ParseInt(record.MinPlayers),
                MaxPlayers = ParseInt(record.MaxPlayers),

                // =========================
                // SHORTS
                // =========================
                MinDuration = ParseShort(record.MinDuration),
                MaxDuration = ParseShort(record.MaxDuration)
            };
        }

        #endregion

        #region ExportToCSV
        public async Task<byte[]> ExportToCsv()
        {
            try
            {
                var games = await context.Games
                    .Include(g => g.Supplier)
                    .Include(g => g.Seller)
                    .Include(g => g.Location)
                    .Include(g => g.Style)
                    .ToListAsync();

                static string BoolToCsv(bool? value)
                {
                    return value switch
                    {
                        true => "true",
                        false => "false",
                        null => "-"
                    };
                }

                static string DecimalToCsv(decimal? value)
                {
                    return value?.ToString(CultureInfo.InvariantCulture) ?? "-";
                }

                static string IntToCsv(int? value)
                {
                    return value?.ToString() ?? "-";
                }

                static string ShortToCsv(short? value)
                {
                    return value?.ToString() ?? "-";
                }

                var exportData = games.Select(g => new GameCsvModel
                {
                    Title = g.Title,
                    Barcode = g.Barcode,

                    LocationName = g.Location?.LocationName,
                    SupplierName = g.Supplier?.SupplierName,
                    SellerName = g.Seller?.SellerName,
                    StyleName = g.Style?.StyleName,

                    ImageLocation = g.ImageLocation,

                    Cost = DecimalToCsv(g.Cost),
                    Weight = DecimalToCsv(g.Weight),

                    NeedsAddedToBgg = BoolToCsv(g.NeedsAddedToBgg),
                    Expansion = BoolToCsv(g.Expansion),

                    DateAcquired = g.DateAcquired?.ToString("M/d/yyyy"),

                    BggGameId = IntToCsv(g.BggGameId),

                    MinPlayers = IntToCsv(g.MinPlayers),
                    MaxPlayers = IntToCsv(g.MaxPlayers),

                    RecommendedPlayers = g.RecommendedPlayers,
                    BestPlayers = g.BestPlayers,

                    MinDuration = ShortToCsv(g.MinDuration),
                    MaxDuration = ShortToCsv(g.MaxDuration)
                }).ToList();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(exportData);
                writer.Flush();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExportToCsv --- " + ex.Message);
                return Array.Empty<byte>();
            }
        }
        #endregion

        #region CheckInByBarcode
        public async Task<CheckInData> CheckInByBarcodeList(List<string> barcodeList)
        {
            CheckInData data = new CheckInData();
            foreach (string barcode in barcodeList)
            {
                int gameId = -1;
                Game? game = null;
                // Grabbing the game associated with the barcode, if possible
                game = await context.Games
                    .FirstOrDefaultAsync(g => g.Barcode == barcode);

                // if the game isn't null, we set the gameid = the query's gameId
                if (game != null)
                {
                    gameId = game.GameId;
                    // Create play record, setting game id to the game we found
                    var timePlayed = new TimePlayed
                    {
                        GameId = gameId,
                        Time = DateOnly.FromDateTime(DateTime.UtcNow)
                    };

                    await context.TimesPlayed.AddAsync(timePlayed);
                    await context.SaveChangesAsync();

                    data.SuccessfulCheckIn.Add(game.Title);
                }
                else
                {
                    data.UnsuccessfulCheckIn.Add(barcode);
                }
            }
            return data;
        }

        #endregion

        #region CheckInByTitle

        public async Task<CheckInData?> CheckInByTitleList(List<string> titleList)
        {
            CheckInData data = new CheckInData();
            foreach (string title in titleList)
            {
                int gameId = -1;
                Game? game = null;
                // Grabbing the game associated with the title
                game = await context.Games
                    .FirstOrDefaultAsync(g => g.Title.ToLower() == title.ToLower());

                // if the game isn't null, we set the gameid = the query's gameId
                if (game != null)
                {
                    gameId = game.GameId;
                    // Create play record, setting game id to the game we found
                    var timePlayed = new TimePlayed
                    {
                        GameId = gameId,
                        Time = DateOnly.FromDateTime(DateTime.UtcNow)
                    };

                    await context.TimesPlayed.AddAsync(timePlayed);
                    await context.SaveChangesAsync();

                    data.SuccessfulCheckIn.Add(game.Title);
                }
                else
                {
                    data.UnsuccessfulCheckIn.Add(title);
                }
            }
            return data;
        }

        #endregion
    }

}
