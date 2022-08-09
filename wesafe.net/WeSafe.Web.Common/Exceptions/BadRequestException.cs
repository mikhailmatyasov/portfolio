using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using WeSafe.Web.Common.Exceptions.Abstract;

namespace WeSafe.Web.Common.Exceptions
{
    public class BadRequestException : Exception, IHttpException
    {
        public int StatusCode { get; } = 400;
        public string GetMessage()
        {
            if (Failures == null || !Failures.Any())
                return Message;

            return string.Join(", ", Failures.Select(x => x.ErrorMessage));
        }

        public IEnumerable<ValidationFailure> Failures { get; } = new List<ValidationFailure>();

        public BadRequestException(IEnumerable<ValidationFailure> failures)
        {
            Failures = failures;
        }

        public BadRequestException(string message) : base(message)
        {
        }
    }
}
