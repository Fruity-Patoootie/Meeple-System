using MeepleSystemAPI.Data;
using MeepleSystemAPI.Models;
using MeepleSystemAPI.IRepository;
using Microsoft.EntityFrameworkCore;

// Keeping this seperate from the Game Data Access Layer
// Has both Login and Register methods
namespace MeepleSystemAPI.Repository
{
    public class UserDAL : IUser // IUser here Abstracts the DAL for Login/Register Endpoints
    {

        // Context object is for direct database access
        private readonly MeepleSystemContext context;

        // Dependency Creation (Controller -> MeeplseSystemContext -> UserDAL)
        public UserDAL(MeepleSystemContext Context)
        {
            context = Context;
        }

        // Login Method (Authenticates a User after Logging In)
        #region Login Method
        /// <summary>
        /// Attempts to authenticate a user based on username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User?> Login(string username, string password)
        {
            try
            {
                // Checks if the user exists in the Big DB
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                // Return Null if the User isn't there
                if (user == null)
                    return null;

                // Check if the password matches
                if (user.Password == password)
                    return user;

                return null; // Password doesn't match, return null
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login --- " + ex.Message);
                return null;
            }
        }
        #endregion

        // Register Method (Creates a new user in the database)
        #region Register Method
        /// <summary>
        /// Registers a new user in the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<SaveUserResponse> Register(User user)
        {
            SaveUserResponse res = new SaveUserResponse(); // User Obj that will go to the User controller

            try
            {
                // Input validation (prevents 
                if (user == null)
                {
                    res.Status = false;
                    res.StatusCode = 400;
                    res.Message = "Invalid user data";
                    return res;
                }

                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                {
                    res.Status = false;
                    res.StatusCode = 400;
                    res.Message = "Username and password are required";
                    return res;
                }

                // Check for duplicate username
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == user.Username.ToLower());

                if (existingUser != null)
                {
                    res.Status = false;
                    res.StatusCode = 409;
                    res.Message = "Username already exists";
                    return res;
                }

                // Save user
                context.Users.Add(user);
                await context.SaveChangesAsync();

                // Remove password before returning
                user.Password = null;

                // Success response
                res.Status = true;
                res.StatusCode = 200;
                res.Message = "User registered successfully";
                res.Data = user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register --- " + ex.Message);

                res.Status = false;
                res.StatusCode = 500;
                res.Message = "An error occurred while registering the user";
            }

            return res;
        }
        #endregion
    }
}