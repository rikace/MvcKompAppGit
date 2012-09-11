using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcAjaxPaging
{
    public static class IEnumerableExtensions
    {
        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}
