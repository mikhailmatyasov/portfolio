using Arch.EntityFrameworkCore.UnitOfWork;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Extensions
{
    /// <summary>
    /// Contains extensions methods to <see cref="IUnitOfWork"/> for getting repositories.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Gets a <see cref="IDeviceRepository"/>.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/> instance.</param>
        /// <returns></returns>
        public static IDeviceRepository GetDeviceRepository(this IUnitOfWork unitOfWork)
        {
            return (IDeviceRepository)unitOfWork.GetRepository<Device>(true);
        }

        /// <summary>
        /// Gets a custom repository of type <see cref="TEntity"/>
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/> instance.</param>
        /// <typeparam name="TRepository">The type of the custom repository <see cref="TRepository"/>.</typeparam>
        /// <typeparam name="TEntity">The entity</typeparam>
        /// <returns></returns>
        public static TRepository GetCustomRepository<TRepository, TEntity>(this IUnitOfWork unitOfWork)
            where TRepository : IRepository<TEntity>
            where TEntity : class
        {
            return (TRepository)unitOfWork.GetRepository<TEntity>(true);
        }
    }
}