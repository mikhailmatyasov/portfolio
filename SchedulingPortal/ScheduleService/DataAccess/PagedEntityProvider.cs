using Microsoft.Extensions.Configuration;
using ProxyReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public abstract class PagedEntityProvider : EntityProviderBase
    {
        private readonly int _maxPageSize;

        protected PagedEntityProvider(IConfiguration configuration)
        {
            _maxPageSize = int.Parse(configuration["AffinityPagedRequestMaxSize"]);
        }

        protected async Task<IEnumerable<TR>> ProcessPagedRequest<T, TR>(T request, Func<T, Task<TR>> apiMethodFunc)
            where T : PagedRequest
            where TR : PagedResponse
        {
            request.pageSize = _maxPageSize;

            var responses = new List<TR>();

            int responseItemsCount;
            var pageIndex = 0;
            do
            {
                request.page = pageIndex++;
                TR response = await apiMethodFunc.Invoke(request);

                responses.Add(response);
                responseItemsCount = GetResponseItemsCount(response);
            }
            while (responseItemsCount >= _maxPageSize);

            return responses;
        }

        protected abstract int GetResponseItemsCount(PagedResponse response);
    }
}
