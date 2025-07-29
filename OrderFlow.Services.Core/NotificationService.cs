using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class NotificationService : BaseRepository, INotificationService
    {

        public NotificationService(DbContext _context) : base(_context)
        {
        }

        public async Task Read(int i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.Id.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsRead)
                {
                    return;
                }

                notification.IsRead = true;
                await this.SaveChangesAsync();
            }
        }

        public async Task SoftDelete(int i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.Id.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsDeleted)
                {
                    return;
                }

                notification.IsDeleted = true;
                await this.SaveChangesAsync();
            }
        }
    }
}
