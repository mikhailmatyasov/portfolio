using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Repositories
{
    /// <inheritdoc cref="IMobileUserRepository"/>
    public class MobileUserRepository : ExtendedRepository<MobileUser>, IMobileUserRepository
    {
        #region Constructors

        public MobileUserRepository(WeSafeDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region IMobileUserRepository implementation

        public Task<MobileUser> FindByPhoneNumberAsync(string phoneNumber, bool disableTracking = false)
        {
            if (String.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException(nameof(phoneNumber));
            }

            var query = GetQueryable(disableTracking);

            return query.FirstOrDefaultAsync(c => c.Phone == phoneNumber);
        }

        #endregion
    }
}