namespace MeepleSystemClient.Utility
{
    public static class ApiConfig
    {
#if DEBUG
        public const string BaseURL = "https://localhost:7142/";
#else
    public const string BaseURL = "https://megatravelapi20240819114355.azurewebsites.net/";
#endif
    }
}

