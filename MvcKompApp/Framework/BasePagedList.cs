using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.Framework
{
    public abstract class BasePagedList<T> : PagedListMetaData, IPagedList<T>, IPagedList, IEnumerable<T>, IEnumerable
    {
        protected List<T> Subset;
        public T this[int index]
        {
            get
            {
                return this.Subset[index];
            }
        }
        public int Count
        {
            get
            {
                return this.Subset.Count;
            }
        }

        protected internal BasePagedList(int pageNumber, int pageSize, int totalItemCount)
        {
            this.Subset = new List<T>();
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", (object)pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", (object)pageSize, "PageSize cannot be less than 1.");
            this.TotalItemCount = totalItemCount;
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            this.PageCount = this.TotalItemCount > 0 ? (int)Math.Ceiling((double)this.TotalItemCount / (double)this.PageSize) : 0;
            this.HasPreviousPage = this.PageNumber > 1;
            this.HasNextPage = this.PageNumber < this.PageCount;
            this.IsFirstPage = this.PageNumber == 1;
            this.IsLastPage = this.PageNumber >= this.PageCount;
            this.FirstItemOnPage = (this.PageNumber - 1) * this.PageSize + 1;
            int num = this.FirstItemOnPage + this.PageSize - 1;
            this.LastItemOnPage = num > this.TotalItemCount ? this.TotalItemCount : num;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)this.Subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        public IPagedList GetMetaData()
        {
            return (IPagedList)new PagedListMetaData((IPagedList)this);
        }
    }

    public interface IPagedList
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        int FirstItemOnPage { get; }
        int LastItemOnPage { get; }
    }

    public interface IPagedList<out T> : IPagedList, IEnumerable<T>, IEnumerable
    {
        T this[int index] { get; }
        int Count { get; }
        IPagedList GetMetaData();
    }

    public class PagedList<T> : BasePagedList<T>
    {
        public PagedList(IQueryable<T> superset, int pageNumber, int pageSize)
            : base(pageNumber, pageSize, superset == null ? 0 : Queryable.Count<T>(superset))
        {
            if (superset == null || this.TotalItemCount <= 0)
                return;
            this.Subset.AddRange(pageNumber == 1 ? (IEnumerable<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(superset, 0), pageSize)) : (IEnumerable<T>)Enumerable.ToList<T>((IEnumerable<T>)Queryable.Take<T>(Queryable.Skip<T>(superset, (pageNumber - 1) * pageSize), pageSize)));
        }

        public PagedList(IEnumerable<T> superset, int pageNumber, int pageSize)
            : base(pageNumber, pageSize, superset == null ? 0 : Enumerable.Count<T>(superset))
        {

            if (superset == null || this.TotalItemCount <= 0)
                return;
            this.Subset.AddRange(pageNumber == 1 ? (IEnumerable<T>)Enumerable.ToList<T>(Enumerable.Take<T>(Enumerable.Skip<T>(superset, 0), pageSize)) : (IEnumerable<T>)Enumerable.ToList<T>(Enumerable.Take<T>(Enumerable.Skip<T>(superset, (pageNumber - 1) * pageSize), pageSize)));
        }
    }


    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int pageNumber, int pageSize)
        {
            return (IPagedList<T>)new PagedList<T>(superset, pageNumber, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            return (IPagedList<T>)new PagedList<T>(superset, pageNumber, pageSize);
        }
    }


    public class PagedListMetaData : IPagedList
    {

        public int PageCount
        {
            get;
            protected set;
        }

        public int TotalItemCount
        {
            get;
            protected set;
        }


        public int PageNumber
        {
            get;
            protected set;
        }

        public int PageSize
        {
            get;
            protected set;
        }


        public bool HasPreviousPage
        {
            get;
            protected set;
        }


        public bool HasNextPage
        {
            get;
            protected set;
        }


        public bool IsFirstPage
        {
            get;
            protected set;
        }


        public bool IsLastPage
        {
            get;
            protected set;
        }


        public int FirstItemOnPage
        {
            get;
            protected set;
        }

        public int LastItemOnPage
        {
            get;
            protected set;
        }


        protected PagedListMetaData()
            : base()
        { }


        public PagedListMetaData(IPagedList pagedList)
            : base()
        {
            this.PageCount = pagedList.PageCount;
            this.TotalItemCount = pagedList.TotalItemCount;
            this.PageNumber = pagedList.PageNumber;
            this.PageSize = pagedList.PageSize;
            this.HasPreviousPage = pagedList.HasPreviousPage;
            this.HasNextPage = pagedList.HasNextPage;
            this.IsFirstPage = pagedList.IsFirstPage;
            this.IsLastPage = pagedList.IsLastPage;
            this.FirstItemOnPage = pagedList.FirstItemOnPage;
            this.LastItemOnPage = pagedList.LastItemOnPage;
        }
    }

    public class StaticPagedList<T> : BasePagedList<T>
    {
        public StaticPagedList(IEnumerable<T> subset, IPagedList metaData)
            : this(
                subset, metaData.PageNumber, metaData.PageSize, metaData.TotalItemCount)
        {
        }

        public StaticPagedList(IEnumerable<T> subset, int pageNumber, int pageSize, int totalItemCount)
            : base(pageNumber, pageSize, totalItemCount)
        {

            this.Subset.AddRange(subset);
        }
    }
}

