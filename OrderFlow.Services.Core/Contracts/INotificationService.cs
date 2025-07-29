using OrderFlow.Data.Repository.Contracts;

namespace OrderFlow.Services.Core.Contracts
{
    public interface INotificationService: IRepository
    {
        Task Read(int id);
        Task SoftDelete(int i);
    }
}
