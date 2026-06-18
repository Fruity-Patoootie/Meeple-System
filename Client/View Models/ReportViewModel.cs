namespace MeepleSystemClient.ViewModels
{
    public class ReportViewModel
    {
        // Filters
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? TimeFrame { get; set; }

        // Results
        public List<ReportItemViewModel> Results { get; set; } = new();
    }

    public class ReportItemViewModel
    {
        public string Title { get; set; }
        public int TimesPlayed { get; set; }
        public string Location { get; set; }
    }
}