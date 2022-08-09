using System;
using WeSafe.Web.Common.Exceptions.Abstract;

namespace WeSafe.Web.Common.Exceptions
{
    public class SystemOperationException : Exception, IHttpException
    {
        public int StatusCode { get; } = 500;

        public SystemOperationException(string message) : base($"System Operation Exception: {message}. Please contact with us.")
        {
        }
        public string GetMessage()
        {
            return Message;
        }
    }
}
