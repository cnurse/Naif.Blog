using System.Collections.Generic;

namespace Naif.Blog.Framework
{
    /// <summary>
    ///   Contains filters that can be applied to <see cref = "IEnumerable" /> stores
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        ///   Filters the incoming store to retrieve pages of a specified size.
        /// </summary>
        /// <typeparam name = "T">The type of the object being filtered</typeparam>
        /// <param name = "source">The source object being filtered</param>
        /// <param name = "pageSize">The page size to use</param>
        /// <returns>
        ///   A <see cref = "PageSelector{T}" /> object that is used to select a single
        ///   page of data from the data source.
        /// </returns>
        public static PageSelector<T> InPagesOf<T>(this IEnumerable<T> source, int pageSize)
        {
            return new PageSelector<T>(source, pageSize);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "source"></param>
        /// <param name = "pageIndex"></param>
        /// <param name = "pageSize"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }
    }
}
