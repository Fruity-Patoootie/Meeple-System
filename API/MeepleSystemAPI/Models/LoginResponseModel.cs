using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class LoginResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public User? User { get; set; }
    }
}