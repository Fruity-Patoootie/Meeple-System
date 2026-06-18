using System.ComponentModel.DataAnnotations;

namespace MeepleSystemClient.ViewModels
{
    public class DeleteGameViewModel
    {
        [Required]
        public int Barcode { get; set; }
    }
}