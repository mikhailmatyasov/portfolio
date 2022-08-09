using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeSafe.Shared;

namespace WeSafe.DAL.Extensions
{
    /// <summary>
    /// Contains extensions methods to <see cref="IQueryable{T}"/> for data querying.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Applies a <see cref="PageRequest"/> to the <see cref="IQueryable{T}"/> for sorting and getting a page of data.
        /// </summary>
        /// <typeparam name="T">The type of query data.</typeparam>
        /// <param name="query"><see cref="IQueryable{T}"/> to apply page request to.</param>
        /// <param name="request">The <see cref="PageRequest"/> to get page.</param>
        /// <returns>A tuple with a page of data and a total number of items in the query.</returns>
        public static async Task<(IQueryable<T> Query, int Total)> ApplyPageRequest<T>(this IQueryable<T> query, PageRequest request)
        {
            if (request.SortBy != null)
            {
                Type type = typeof(T);
                ParameterExpression parameter = Expression.Parameter(type, "c");
                bool first = true;
                var properties = type.GetProperties();

                foreach (var order in request.SortBy)
                {
                    var member = properties.FirstOrDefault(c => c.Name.ToLower() == order.Key);//type.GetProperty(order.Key);

                    if (member == null) continue;

                    var propertyAccess = Expression.MakeMemberAccess(parameter, member);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    var resultExp = Expression.Call(typeof(Queryable),
                        first
                            ? (order.Value == OrderType.Asc ? "OrderBy" : "OrderByDescending")
                            : (order.Value == OrderType.Asc ? "ThenBy" : "ThenByDescending"),
                        new[] { type, propertyAccess.Type },
                        query.Expression, orderByExp);

                    query = query.Provider.CreateQuery<T>(resultExp);
                    first = false;
                }
            }

            int total = await query.CountAsync();

            if (request.Skip.HasValue) query = query.Skip(request.Skip.Value);
            if (request.Take.HasValue) query = query.Take(request.Take.Value);

            return (query, total);
        }
    }
}
