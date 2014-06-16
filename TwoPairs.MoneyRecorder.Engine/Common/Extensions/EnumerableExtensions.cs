using System;
using System.Collections.Generic;
using System.Linq;

namespace TwoPairs.MoneyRecorder
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Sort<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, SortType sortType)
        {
            return sortType == SortType.Asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }

        public static IOrderedEnumerable<T> Sort<T, TKey>(this IQueryable<T> source, Func<T, TKey> selector, SortType sortType)
        {
            return sortType == SortType.Asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }
    }

    public enum SortType
    {
        Asc,
        Desc
    }
}