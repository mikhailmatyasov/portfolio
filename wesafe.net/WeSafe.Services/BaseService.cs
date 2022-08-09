using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;

namespace WeSafe.Services
{
    public abstract class BaseService
    {
        protected BaseService(WeSafeDbContext context, ILoggerFactory loggerFactory)
        {
            DbContext = context;
            Logger = loggerFactory.CreateLogger(GetType());
        }

        protected WeSafeDbContext DbContext { get; }

        protected ILogger Logger { get; }

        protected virtual Task SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}