using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MVCControlsToolkit.Linq
{
    public static class OrderHelpers
    {
        public static IQueryable<TElement> ApplyOrder<TElement>(this IQueryable<TElement> source,
            IEnumerable<KeyValuePair<LambdaExpression, OrderType>> orders)
        {
            
            foreach (KeyValuePair<LambdaExpression, OrderType> order in orders)
            {
                IOrderedQueryable<TElement> ordSource = source as IOrderedQueryable<TElement>;
                if (ordSource == null || !(ordSource is IOrderedQueryable<TElement>))
                {
                    if (order.Value == OrderType.Ascending)
                    {
                        source=
                            typeof(Queryable).GetMethods().Single(
                                method => method.Name == "OrderBy"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key }) as IQueryable<TElement>;
                    }
                    else
                    {
                        source =
                            typeof(Queryable).GetMethods().Single(
                                method => method.Name == "OrderByDescending"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key }) as IQueryable<TElement>;
                    }
                }
                else
                {
                    try
                    {

                        if (order.Value == OrderType.Ascending)
                        {
                            source =
                                typeof(Queryable).GetMethods().Single(
                                    method => method.Name == "ThenBy"
                                    && method.IsGenericMethodDefinition
                                    && method.GetGenericArguments().Length == 2
                                    && method.GetParameters().Length == 2).MakeGenericMethod(
                                       typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { ordSource, order.Key }) as IQueryable<TElement>;
                        }
                        else
                        {
                            source =
                                typeof(Queryable).GetMethods().Single(
                                    method => method.Name == "ThenByDescending"
                                    && method.IsGenericMethodDefinition
                                    && method.GetGenericArguments().Length == 2
                                    && method.GetParameters().Length == 2).MakeGenericMethod(
                                       typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { ordSource, order.Key }) as IQueryable<TElement>;
                        }
                    }
                    catch
                    {
                        if (order.Value == OrderType.Ascending)
                        {
                            source =
                                typeof(Queryable).GetMethods().Single(
                                    method => method.Name == "OrderBy"
                                    && method.IsGenericMethodDefinition
                                    && method.GetGenericArguments().Length == 2
                                    && method.GetParameters().Length == 2).MakeGenericMethod(
                                       typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key }) as IQueryable<TElement>;
                        }
                        else
                        {
                            source =
                                typeof(Queryable).GetMethods().Single(
                                    method => method.Name == "OrderByDescending"
                                    && method.IsGenericMethodDefinition
                                    && method.GetGenericArguments().Length == 2
                                    && method.GetParameters().Length == 2).MakeGenericMethod(
                                       typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key }) as IQueryable<TElement>;
                        }
                    }
                }
            }
            return source;
        }
        public static IEnumerable<TElement> ApplyOrder<TElement>(this  IEnumerable<TElement> source,
            IEnumerable<KeyValuePair<LambdaExpression, OrderType>> orders)
        {
            foreach (KeyValuePair<LambdaExpression, OrderType> order in orders)
            {
                IOrderedEnumerable<TElement> ordSource = source as IOrderedEnumerable<TElement>;
                if (ordSource == null)
                {
                    if (order.Value == OrderType.Ascending)
                    {
                        source =
                            typeof(Enumerable).GetMethods().Single(
                                method => method.Name == "OrderBy"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key.Compile() }) as IEnumerable<TElement>;
                    }
                    else
                    {
                        source =
                            typeof(Enumerable).GetMethods().Single(
                                method => method.Name == "OrderByDescending"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { source, order.Key.Compile() }) as IEnumerable<TElement>;
                    }
                }
                else
                {
                    if (order.Value == OrderType.Ascending)
                    {
                        source =
                            typeof(Enumerable).GetMethods().Single(
                                method => method.Name == "ThenBy"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { ordSource, order.Key.Compile() }) as IEnumerable<TElement>;
                    }
                    else
                    {
                        source =
                            typeof(Enumerable).GetMethods().Single(
                                method => method.Name == "ThenByDescending"
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2).MakeGenericMethod(
                                   typeof(TElement), order.Key.ReturnType).Invoke(null, new object[] { ordSource, order.Key.Compile() }) as IEnumerable<TElement>;
                    }
                }
            }
            return source;
        }
    } 
}
