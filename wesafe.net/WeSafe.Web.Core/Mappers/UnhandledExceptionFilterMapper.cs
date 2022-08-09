using System;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Mappers
{
    public class UnhandledExceptionFilterMapper
    {
        public UnhandledExceptionRecordQuery ToUnhandledExceptionRecordQuery(UnhandledExceptionFilter unhandledExceptionFilter)
        {
            if (unhandledExceptionFilter == null)
                throw new ArgumentNullException(nameof(unhandledExceptionFilter));

            return new UnhandledExceptionRecordQuery()
            {
                SearchText = unhandledExceptionFilter.SearchText,
                UserName = unhandledExceptionFilter.UserName,
                FromDate = unhandledExceptionFilter.FromDate,
                ToDate = unhandledExceptionFilter.ToDate,
                Skip = unhandledExceptionFilter.Skip,
                Take = unhandledExceptionFilter.Take
            };
        }
    }
}
