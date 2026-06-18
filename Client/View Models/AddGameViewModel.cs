using System.ComponentModel.DataAnnotations;

namespace MeepleSystemClient.ViewModels
{
    public class AddGameViewModel
    {
        [Required]
        public string Title { get; set; }

        [Range(1, 20)]
        public int Players { get; set; }

        [Range(0, 10)]
        public decimal Weight { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public int Barcode { get; set; }

        // 🔽 Optional (for future dropdowns)
        public List<CategoryOption> Categories { get; set; } = new();
        public List<int> SelectedCategoryIds { get; set; } = new();
    }

    public class CategoryOption
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}