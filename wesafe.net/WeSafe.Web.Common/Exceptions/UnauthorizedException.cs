using System;
using WeSafe.Web.Common.Exceptions.Abstract;

namespace WeSafe.Web.Common.Exceptions
{
    public class UnauthorizedException : Exception, IHttpException
    {
        public int StatusCode { get; } = 401;

        public UnauthorizedException(string message) : base(message)
        {

        }
        public string GetMessage()
        {
            return Message;
        }
    }
}
