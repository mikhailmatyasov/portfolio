using System;
using WeSafe.Logger.Abstraction.Enums;

namespace WeSafe.Logger.Abstraction.Interfaces
{
    public interface IWeSafeLog
    {
        WeSafeLogLevel LogLevel { get; set; }

        string Message { get; set; }

        DateTime DateTime { get; set; }
    }
}
