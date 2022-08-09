using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WeSafe.DAL.Repositories.Extended
{
    public class ExtendedRepository<TEntity> : Repository<TEntity>, IExtendedRepository<TEntity> where TEntity : class
    {
        public ExtendedRepository(DbContext dbContext) : base(dbContext)
        {
        }

        #region IExtendedRepository

        public async Task<IEnumerable<TResult>> Get<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false) where TResult : class
        {
            var query = GetQueryable(disableTracking);

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await query.Select(selector).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        #endregion

        #region Protected Methods

        protected IQueryable<TEntity> GetQueryable(bool disableTracking)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = _dbSet.AsNoTracking();
            }

            return query;
        }

        #endregion
    }
}
