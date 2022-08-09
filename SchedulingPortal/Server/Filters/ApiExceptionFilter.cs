using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ScheduleService.Exceptions;
using System;
using System.Collections.Generic;

namespace Server.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(AffinityNotFoundException), HandleAffinityNotFoundException },
                { typeof(AffinityServerErrorException), HandleAffinityServerErrorException },
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = "Internal server error",
            };
            context.ExceptionHandled = true;
        }

        private void HandleAffinityNotFoundException(ExceptionContext context)
        {
            context.Result = new NotFoundObjectResult(context.Exception.Message);
            context.ExceptionHandled = true;
        }

        private void HandleAffinityServerErrorException(ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = "Affinity API error",
            };
            context.ExceptionHandled = true;
        }
    }
}
