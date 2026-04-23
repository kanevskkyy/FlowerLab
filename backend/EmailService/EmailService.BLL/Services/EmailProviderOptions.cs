namespace EmailService.BLL.Service
{
    public class EmailProviderOptions
    {
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string? SmtpPassword { get; set; }
    }
}
