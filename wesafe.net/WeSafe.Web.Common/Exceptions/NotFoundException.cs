using System;
using WeSafe.Web.Common.Exceptions.Abstract;

namespace WeSafe.Web.Common.Exceptions
{
    public class NotFoundException : Exception, IHttpException
    {
        public int StatusCode { get; } = 404;
        public string GetMessage()
        {
            return Message;
        }

        public NotFoundException(string message) : base(message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new InvalidOperationException("The error message can't be generated.");
        }
    }
}
