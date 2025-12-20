namespace Flowdash_Mini.ViewModels.Settings
{
    public class AppSettingSmtpVM
    {
        public string SmtpEmail { get; set; } = string.Empty;
        public string SmtpName { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public bool SmtpSsl { get; set; }
    }
}
