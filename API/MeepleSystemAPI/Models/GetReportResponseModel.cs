namespace MeepleSystemAPI.Models
{
    public class GetReportResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; } = null;
        public List<ReportGameModel>? gameList { get; set; } = null;
    }
}
