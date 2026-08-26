namespace IMODY.Configuration
{
    public class AppSettings
    {
        public GeminiSettings Gemini { get; set; } = new();
    }

    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}