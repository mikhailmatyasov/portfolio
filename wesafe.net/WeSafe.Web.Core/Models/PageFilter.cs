namespace WeSafe.Web.Core.Models
{
    public class PageFilter
    {
        /// <summary>
        /// Skip specified number of rows. Can be null
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Take specified number of rows from data array. Can be null.
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Sort string in format 'fieldName1-asc;fieldName2-desc'
        /// </summary>
        public string SortBy { get; set; }
    }
}