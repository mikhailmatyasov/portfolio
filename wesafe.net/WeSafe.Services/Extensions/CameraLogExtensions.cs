using System.Collections.Generic;
using System.Linq;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Extensions
{
    public static class CameraLogExtensions
    {
        public static IQueryable<CameraLog> ApplyEventsFilterQuery(this IQueryable<CameraLog> query, EventBaseRequest request)
        {
            switch (request)
            {
                case EventSearchRequest eventSearchRequest:
                    {
                        if (eventSearchRequest.CameraIds != null && eventSearchRequest.CameraIds.Any())
                            query = query.Where(x => eventSearchRequest.CameraIds.Contains(x.CameraId));

                        if (eventSearchRequest.DeviceIds != null && eventSearchRequest.DeviceIds.Any())
                            query = query.Where(c => eventSearchRequest.DeviceIds.Contains(c.Camera.DeviceId));

                        break;
                    }

                case EventRequest eventRequest:
                    {
                        if (eventRequest.CameraId != null)
                            query = query.Where(x => eventRequest.CameraId == x.CameraId);

                        if (eventRequest.DeviceId != null)
                            query = query.Where(c => eventRequest.DeviceId == c.Camera.DeviceId);

                        break;
                    }

                default:
                    break;
            }

            return query.OrderByDescending(c => c.Id).Skip(request.Skip.Value).Take(request.Take.Value);
        }

        public static IQueryable<CameraLog> ApplyDateFilterQuery(this IQueryable<CameraLog> query, EventBaseRequest request)
        {
            if (request.FromDate.HasValue)
                query = query.Where(c => c.Time >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(c => c.Time <= request.ToDate);

            return query;
        }

        public static IEnumerable<CameraLog> ApplyDateFilterQuery(this IEnumerable<CameraLog> query, EventBaseRequest request)
        {
            if (request.FromDate.HasValue)
                query = query.Where(c => c.Time >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(c => c.Time <= request.ToDate);

            return query;
        }
    }
}