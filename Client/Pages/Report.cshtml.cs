using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ReportModel : PageModel
{
    private readonly IGameService _gameService;

    public List<ReportGame> Games { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public ReportModel(IGameService gameService)
    {
        _gameService = gameService;
    }

    // FILTER PROPERTIES
    [BindProperty(SupportsGet = true)]
    public int Timeframe { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Most { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Quantity { get; set; }

    // CSV UPLOAD
    [BindProperty]
    public IFormFile? UploadFile { get; set; }

    // GET
    public async Task OnGetAsync(bool generated = false)
    {
        if (generated)
        {
            try
            {
                Games = await _gameService.GetGameReportAsync(
                Timeframe,
                Most,
                Quantity ?? int.MaxValue);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }

    // GENERATE REPORT
    public IActionResult OnPost()
    {
        return RedirectToPage("/Report", new
        {
            timeframe = Timeframe,
            most = Most,
            quantity = Quantity,
            generated = true
        });
    }

    // CSV UPLOAD
    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (UploadFile == null || UploadFile.Length == 0)
        {
            ErrorMessage = "Please select a CSV file.";
            return Page();
        }

        try
        {
            using var stream = UploadFile.OpenReadStream();
            var success = await _gameService.UploadCsvAsync(stream, UploadFile.FileName);

            if (!success)
            {
                ErrorMessage = "CSV upload failed.";
                return Page();
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    // EXPORT CSV
    public async Task<IActionResult> OnPostExportAsync()
    {
        try
        {
            var fileBytes = await _gameService.ExportCsvAsync();

            if (fileBytes == null)
            {
                ErrorMessage = "Export failed.";
                return Page();
            }

            return File(fileBytes, "text/csv", "games_export.csv");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}