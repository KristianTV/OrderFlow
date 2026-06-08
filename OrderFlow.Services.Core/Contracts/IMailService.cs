namespace OrderFlow.Services.Contracts
{
    public interface IMailService
    {
        public Task SendMail(string to, string subject, string body);
    }
}
