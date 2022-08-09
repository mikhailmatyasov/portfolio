using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface IUnhandledExceptionsSender
    {
        Task SendUnhandledExceptionsAsync(IEnumerable<UnhandledExceptionModel> unhandledExceptions);
    }
}
