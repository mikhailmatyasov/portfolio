using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Repositories
{
    /// <inheritdoc cref="ISubscriberRepository"/>
    public class SubscriberRepository : ExtendedRepository<ClientSubscriber>, ISubscriberRepository
    {
        #region Constructors

        public SubscriberRepository(WeSafeDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region ISubscriberRepository implementation

        public Task<bool> HasActiveClientsAsync(string phone)
        {
            if (String.IsNullOrEmpty(phone))
            {
                throw new ArgumentNullException(nameof(phone));
            }

            return _dbSet.AnyAsync(c => c.IsActive && c.Phone == phone);
        }

        public Task<bool> AnyAsync(int clientId, string phone)
        {
            if ( String.IsNullOrEmpty(phone) )
            {
                throw new ArgumentNullException(nameof(phone));
            }

            return _dbSet.AnyAsync(c => c.Phone == phone && c.ClientId == clientId);
        }

        #endregion
    }
}