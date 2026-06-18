// Purpose:
// Wraps a list of games from the API response

using System.Text.Json.Serialization;

namespace MeepleSystemClient.Models
{
    public class DeleteGameResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public string? GameTitle { get; set; }

        public bool SellerDeleted { get; set; }
        public bool SupplierDeleted { get; set; }
        public bool LocationDeleted { get; set; }
    }
}