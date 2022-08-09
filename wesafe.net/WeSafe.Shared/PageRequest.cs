using System;
using System.Collections.Generic;

namespace WeSafe.Shared
{
    /// <summary>
    /// Provides parameters to make a page request from an array of data.
    /// </summary>
    public class PageRequest
    {
        private int? _skip;
        private int? _take;

        /// <summary>
        /// Search text to filter data. Can be null.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Skip specified number of rows. Can be null
        /// </summary>
        public int? Skip
        {
            get => _skip;
            set
            {
                if (value < 0) value = 0;

                _skip = value;
            }
        }

        /// <summary>
        /// Take specified number of rows from data array. Can be null.
        /// </summary>
        public int? Take
        {
            get => _take;
            set
            {
                if (value < 0) value = 0;

                _take = value;
            }
        }

        /// <summary>
        /// Sorting parameters to data array as pairs of field name and order type.
        /// </summary>
        public Dictionary<string, OrderType> SortBy { get; set; }

        /// <summary>
        /// Parses sort string in format 'fieldName1-asc;fieldName2-desc' to sort dictionary.
        /// </summary>
        /// <param name="sort">Sort string.</param>
        /// <returns>A dictionary as pairs of field name and order type to use in SortBy property.</returns>
        public static Dictionary<string, OrderType> ParseSort(string sort)
        {
            var result = new Dictionary<string, OrderType>();

            if (String.IsNullOrEmpty(sort)) return result;

            var orders = sort.Split(';');

            foreach (var order in orders)
            {
                var desc = order.Split('-');

                if (desc.Length == 0) continue;

                string key = desc[0].ToLower();

                result.Add(key, desc.Length > 1 ? (desc[1] == "asc" ? OrderType.Asc : OrderType.Desc) : OrderType.Asc);
            }

            return result;
        }
    }
}