namespace MeepleSystemAPI.Models
{
    public class CheckInData
    {
        public List<string> SuccessfulCheckIn { get; set; }
        public List<string> UnsuccessfulCheckIn { get; set; }

        public CheckInData()
        {
            SuccessfulCheckIn = new List<string>();
            UnsuccessfulCheckIn = new List<string>();
        }

    }
}
