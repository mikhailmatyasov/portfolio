using System;
using System.Linq;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Repositories
{
    /// <inheritdoc cref="IClientRepository"/>
    public class ClientRepository : ExtendedRepository<Client>, IClientRepository
    {
        #region Constructors

        public ClientRepository(WeSafeDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region IClientRepository implementation

        public Task<Client> FindByPhoneAsync(string phone, bool disableTracking = false)
        {
            if ( String.IsNullOrEmpty(phone) )
            {
                throw new ArgumentNullException(nameof(phone));
            }

            var query = GetQueryable(disableTracking);

            return query.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        #endregion
    }
}