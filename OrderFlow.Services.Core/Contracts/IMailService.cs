namespace OrderFlow.Services.Contracts
{
    public interface IMailService
    {
        public Task SendMailAsync(string to, string subject, string body);
    }
}
