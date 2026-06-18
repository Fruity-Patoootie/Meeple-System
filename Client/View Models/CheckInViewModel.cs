namespace MeepleSystemClient.ViewModels
{
    public class CheckInViewModel
    {
        public int Barcode { get; set; }

        // What shows on screen after scan
        public List<GameDisplayViewModel> Results { get; set; } = new();
    }

    public class GameDisplayViewModel
    {
        public string Title { get; set; }
        public int Barcode { get; set; }
        public string Location { get; set; }
    }
}
