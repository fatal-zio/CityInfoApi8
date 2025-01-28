namespace CityInfo.Api.Services
{
    public class LocalMailService(IConfiguration configuration) : IMailService
    {
        private readonly string _mailTo = configuration["mailSettings:mailToAddress"] ?? "default@mycompany.com";
        private readonly string _mailFrom = configuration["mailSettings:mailFromAddress"] ?? "default@mycompany.com";

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
