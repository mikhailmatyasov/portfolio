using System;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Extensions
{
    public static class EventRequestExtensions
    {
        public static void Prepare(this EventBaseRequest request)
        {
            if (request.FromDate.HasValue)
                request.FromDate = DateTime.SpecifyKind(request.FromDate.Value, DateTimeKind.Utc);

            if (request.ToDate.HasValue)
                request.ToDate = DateTime.SpecifyKind(request.ToDate.Value, DateTimeKind.Utc);

            if (!request.Skip.HasValue)
                request.Skip = 0;

            if (!request.Take.HasValue)
                request.Take = 25;
        }

        public static void Validate(this EventBaseRequest request, int limit = 100)
        {
            if (request.Skip < 0)
                throw new ArgumentOutOfRangeException($"The value {request.Skip} can not be 0");

            if (request.Take <= 0)
                throw new ArgumentOutOfRangeException($"The value {request.Take} can not be 0");

            // [EM]: I think requesting more than 100 records will negatively impact performance.
            if (request.Take > limit)
                throw new ArgumentOutOfRangeException(nameof(request.Take),
                    $"You cannot take more {limit} events per request");
        }
    }
}