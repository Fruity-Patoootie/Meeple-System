using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class SaveUserResponse
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public User? Data { get; set; }
    }
}
