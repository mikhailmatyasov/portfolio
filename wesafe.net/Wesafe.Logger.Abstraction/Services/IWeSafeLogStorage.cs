using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Interfaces;

namespace WeSafe.Logger.Abstraction.Services
{
    public interface IWeSafeLogStorage
    {
        Task Add(IEnumerable<IWeSafeLog> logs);
    }
}
