using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeepleSystemClient.Pages
{
    public class AddGameFromCSVModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddGameFromCSVModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload == null || Upload.Length == 0)
            {
                IsSuccess = false;
                Message = "Please select a CSV file.";
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                using var content = new MultipartFormDataContent();

                await using var stream = Upload.OpenReadStream();

                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

                content.Add(fileContent, "file", Upload.FileName);

                var response = await client.PostAsync(
                    "https://localhost:7049/Game/InsertFromCsv",
                    content);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    IsSuccess = true;
                    Message = "CSV imported successfully.";
                }
                else
                {
                    IsSuccess = false;
                    Message = $"Upload failed: {response.StatusCode}";
                }

                return Page();
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}