using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using OrderFlow.Services.Contracts;

namespace OrderFlow.Services
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly MailboxAddress _from;

        private readonly string? _host;
        private readonly int _port;
        private readonly string? _mail;
        private readonly string? _password;


        public MailService(ILogger<MailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _host = _configuration.GetValue<string>("MailSettings:SmtpServer");
            _port = _configuration.GetValue<int>("MailSettings:SmtpPort");
            _mail = _configuration.GetValue<string>("MailSettings:SmtpUsername");
            _password = _configuration.GetValue<string>("MailSettings:SmtpPassword");

            if (string.IsNullOrEmpty(_host) || string.IsNullOrEmpty(_mail) || string.IsNullOrEmpty(_password))
                throw new InvalidOperationException("Mail settings are not properly configured.");

            _from = new MailboxAddress("OrderFlow", _mail);
        }

        public async Task SendMail(string to, string subject, string body)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(_from);
            message.To.Add(MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using (var writer = new SmtpClient())
            {
                await writer.ConnectAsync(_host!, _port);
                await writer.AuthenticateAsync(_mail!, _password!);
                await writer.SendAsync(message);
                await writer.DisconnectAsync(true);
            }

        }
    }
}
