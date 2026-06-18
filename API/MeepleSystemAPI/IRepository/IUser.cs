
using MeepleSystemAPI.Data;
using MeepleSystemAPI.Models;

// Controller -> IUser -> UserDAL -> Database

namespace MeepleSystemAPI.IRepository
{
    public interface IUser
    {
        /// <summary>
        /// Attempts to authenticate a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<User?> Login(string username, string password);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<SaveUserResponse> Register(User user); //Register User Endpoint
    }
}
