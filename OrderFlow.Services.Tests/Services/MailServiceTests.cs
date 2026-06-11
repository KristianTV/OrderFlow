using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using OrderFlow.Services;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class MailServiceTests
    {
        [Test]
        public void Constructor_Throws_WhenConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MailService(Mock.Of<ILogger<MailService>>(), null!));
        }

        [Test]
        public void Constructor_Throws_WhenMailSettingsAreMissing()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            Assert.Throws<InvalidOperationException>(() => new MailService(Mock.Of<ILogger<MailService>>(), configuration));
        }

        [Test]
        public void SendMailAsync_ThrowsParseException_WhenRecipientIsInvalid()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["MailSettings:SmtpServer"] = "localhost",
                    ["MailSettings:SmtpPort"] = "25",
                    ["MailSettings:SmtpUsername"] = "sender@test.com",
                    ["MailSettings:SmtpPassword"] = "password"
                })
                .Build();
            var service = new MailService(Mock.Of<ILogger<MailService>>(), configuration);

            Assert.ThrowsAsync<ParseException>(() => service.SendMailAsync("not an email", "Subject", "Body"));
        }
    }
}
