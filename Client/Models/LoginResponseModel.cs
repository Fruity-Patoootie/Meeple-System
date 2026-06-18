namespace MeepleSystemClient.Models
{
    public class LoginResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public UserData? Data { get; set; }
    }

    public class UserData
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}