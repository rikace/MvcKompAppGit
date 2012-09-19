using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.DataAnnotations;
using System.Web.Mvc;
using System.Reflection;

namespace MVCControlsToolkit.Controls
{
    public class SortInfo<T>: IDisplayModel
    {

        public string SortInfoAsString { get; set; }
        public object ExportToModel(Type TargetType, params object[] context)
        {
            List<KeyValuePair<LambdaExpression, OrderType>> res = new List<KeyValuePair<LambdaExpression, OrderType>>();
            if (string.IsNullOrWhiteSpace(SortInfoAsString)) return res;
            string[] orders = SortInfoAsString.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
            Type type = typeof(T);
            foreach (string order in orders)
            {
                if (string.IsNullOrWhiteSpace(order)) continue;
                ParameterExpression arg = Expression.Parameter(type, "x");
                Expression expr = arg;
                string[] pair = order.Trim().Split('#');
                pair[1] = pair[1].Trim();
                if (pair[1].Length == 0) continue;
                PropertyInfo pi = type.GetProperty(pair[0]);
                if (pi == null) continue;
                PropertyAccessor pa = new PropertyAccessor(pair[0], type);
                CanSortAttribute[] cs = pa[typeof(CanSortAttribute)] as CanSortAttribute[];
                if (cs == null || cs.Length == 0) throw new NotAllowedColumnException(pair[0], type);
                expr = Expression.Property(expr, pi);
                Type delegateType = typeof(Func<,>).MakeGenericType(type, pi.PropertyType);
                LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
                OrderType orderType = pair[1].Contains('-') ? OrderType.Descending : OrderType.Ascending;
                res.Add(new KeyValuePair<LambdaExpression, OrderType>(lambda, orderType));
            }
            return res;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            IEnumerable<KeyValuePair<LambdaExpression, OrderType>> orders = model as IEnumerable<KeyValuePair<LambdaExpression, OrderType>>;
            if (orders == null)
            {
                SortInfoAsString = string.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
  
            foreach (KeyValuePair<LambdaExpression, OrderType> order in orders)
            {
               
                string currExp = ExpressionHelper.GetExpressionText(order.Key).Trim();
                if (currExp.Length != 0)
                {
                    sb.Append(' ');
                    sb.Append(currExp);
                    sb.Append('#');
                    if (order.Value == OrderType.Ascending)
                        sb.Append('+');
                    else
                        sb.Append('-');
                    sb.Append(';');

                }
            }
            SortInfoAsString = sb.ToString();
        }
    }
}
