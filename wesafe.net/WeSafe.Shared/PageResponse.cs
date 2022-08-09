using System.Collections.Generic;

namespace WeSafe.Shared
{
    /// <summary>
    /// Provides a filtered data as a result of making page request with PageRequest class.
    /// </summary>
    /// <typeparam name="TResult">The type of data in result array.</typeparam>
    public class PageResponse<TResult>
    {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        public PageResponse()
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="items"><see cref="IEnumerable{TResult}"/> as result of data.</param>
        /// <param name="total">Total amount of data.</param>
        public PageResponse(IEnumerable<TResult> items, int total)
        {
            Items = items;
            Total = total;
        }

        /// <summary>
        /// Result array of filtered data.
        /// </summary>
        public IEnumerable<TResult> Items { get; set; }

        /// <summary>
        /// Total amount of data.
        /// </summary>
        public int Total { get; set; }
    }
}