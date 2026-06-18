namespace MeepleSystemClient.Models
{
    public class SaveUserResponse
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }
}