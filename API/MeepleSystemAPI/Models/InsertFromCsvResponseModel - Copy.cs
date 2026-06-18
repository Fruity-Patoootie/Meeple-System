using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class InsertFromCsvResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }

        public List<Game>? Games { get; set; }
    }
}