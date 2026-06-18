using Microsoft.AspNetCore.Mvc;
using MeepleSystemAPI.Data;
using MeepleSystemAPI.IRepository;
using MeepleSystemAPI.Repository;
using MeepleSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;

// Client -> UserController -> IUser -> UserDAL (implementation) -> MeepleSystemContext (the database context)



namespace MeepleSystemAPI.Controllers
{
    public class UserController : ControllerBase // ControllerBase is specific to APIs as to prevent View controller use
    {
        // Depndencies (Fields)
        // 
        // Data Access Layer (UserDAL) for User operations
        private readonly IUser repository;

        // Context object is for direct database access
        private readonly MeepleSystemContext context;

        // Reads appsettings.json for connection string/configuration values
        private IConfiguration config;

        // Dependency Creation
        public UserController(IConfiguration Config)
        {
            // Recieves config from ASP.NET Core's dependency injection system 
            config = Config;
            // Creates a DB context 
            context = new MeepleSystemContext(config);
            // Creates a repository (UserDAL) for user operations
            repository = new UserDAL(context);
        }

        // POST Method for LoginUser Endpoint
        #region Login Endpoint

        [HttpPost("Login", Name = "Login")] //Http routing
        [AllowAnonymous] // No Authorization needed
        public async Task<LoginResponseModel> Login([FromBody] LoginModel model)
        {
            LoginResponseModel response = new LoginResponseModel(); // Response Object

            try
            {

                // Input Validation
                if (model != null && !string.IsNullOrEmpty(model.Username))
                {
                    var user = await repository.Login(model.Username, model.Password); // Repository call to login user (Jumps to UserDAL)

                    // User found/Password matched
                    if (user != null)
                    {
                        // remove password before returning
                        user.Password = null;

                        response.Status = true;
                        response.StatusCode = 200;
                        response.Message = "Login successful";
                        response.User = user;
                    }

                    // Failed User found or Password matched
                    else
                    {
                        response.Status = false;
                        response.StatusCode = 401;
                        response.Message = "Invalid username or password";
                    }
                }

                // Bad Request (Input was messed up)
                else
                {
                    response.Status = false;
                    response.StatusCode = 400;
                    response.Message = "Invalid request";
                }
            }

            // Exception Handling (Something went wrong in the UserDAL or DB)
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = 500;
                response.Message = "Login failed";
                Console.WriteLine(ex.Message);
            }

            return response; // Returns response to client in JSON
        }
        #endregion

        // POST Method for RegisterUser Endpoint
        #region Register Endpoint
        [HttpPost("Register", Name = "Register")] // HTTP Routing
        [AllowAnonymous] // No Authorization needed (!!!MIGHT NEED TO CHANGED THIS!!!)
        public async Task<SaveUserResponse> Register([FromBody] User user)
        {
            SaveUserResponse response = new SaveUserResponse(); // Response Object

            try
            {
                var result = await repository.Register(user); // Goes to UserDAL (registers the user in the database)

                // Registration was Successful, Return response to Client
                if (result.Status)
                {
                    response = result;
                }

                // Response set to error and still Returned response to Client
                else
                {
                    response.Status = false;
                    response.StatusCode = result.StatusCode;
                    response.Message = result.Message;
                }
            }

            // Exception Handling (Something went wrong in the UserDAL or Database)
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = 500;
                response.Message = "Register failed";
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        #endregion
    }
}