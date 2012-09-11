using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcPaging
{
    public static class IQueryableExtensions
    {
        #region IQueryable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}
