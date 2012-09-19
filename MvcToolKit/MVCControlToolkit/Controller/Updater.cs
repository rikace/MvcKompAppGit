using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Collections;

namespace MVCControlsToolkit.Controller
{
    
    public static class  ControllerUtilities{

        private static void applyToDateRec(Func<DateTime, DateTime> f, object x, object father, string propertyName)
        {
            if (x == null) return;
            if (x is IEnumerable)
            {

                foreach (object y in x as IEnumerable)
                {
                    applyToDateRec(f, y, null, null);

                }
            }
            else
            {
                Type currType = x.GetType();
                if (currType.IsClass)
                {
                    foreach (PropertyInfo prop in currType.GetProperties())
                    {

                        if ((prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)) && prop.CanRead && prop.CanWrite)
                        {
                            object dateVal = prop.GetValue(x, new object[0]);
                            if (dateVal != null)
                            {
                                prop.SetValue(x, f((DateTime)dateVal), new object[0]);
                            }

                        }
                        else applyToDateRec(f, prop.GetValue(x, new object[0]),
                           x,
                           prop.Name);
                    }
                }
            }
        }
        
        public static void DatesToLocale<T>(T x, bool round=false)
        {
            if (round) applyToDateRec(y => RoundDateTimeToDate(y.ToLocalTime()), x, null, null);
            else applyToDateRec(y => y.ToLocalTime(), x, null, null);
        }
        public static void DeclareAllDatesKind<T>(T x, DateTimeKind kind, bool round = false)
        {
            if (round) applyToDateRec(y => RoundDateTimeToDate(DeclareDateTimeKind(y, kind)), x, null, null);
            else applyToDateRec(y => DeclareDateTimeKind(y, kind), x, null, null);
        }
        public static void DatesToUTC<T>(T x, bool round=false)
        {
            if (round) applyToDateRec(y => RoundDateTimeToDate(y.ToUniversalTime()), x, null, null);
            else applyToDateRec(y => y.ToUniversalTime(), x, null, null);
        }  
        public static DateTime DeclareDateTimeKind (DateTime date, DateTimeKind kind)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, kind);
        }
        public static DateTime? DeclareDateTimeKind(DateTime? date, DateTimeKind kind)
        {
            if (!date.HasValue) return null;
            return DeclareDateTimeKind (date.Value, kind);
        }
        public static void TransformDates<T>(Func<DateTime, DateTime> f, T x)
        {
            applyToDateRec(f, x, null, null);
        }
        public static void RoundDateTimesToDates<T>(T x)
        {
            applyToDateRec(y => ControllerUtilities.RoundDateTimeToDate(y), x, null, null);
        }
        public static DateTime RoundDateTimeToDate(DateTime x)
        {
            if (x > new DateTime(x.Year, x.Month, x.Day, 11, 59, 59, 999, x.Kind)) return new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, 0, x.Kind).AddDays(1.0);
            return new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, 0, x.Kind) ;
            
        }
        public static DateTime? RoundDateTimeToDate(DateTime? x)
        {
            if (!x.HasValue) return x;
            else return RoundDateTimeToDate(x.Value);

        }
    }
    public class Updater<T, U>
    {
        public List<T> Modified { get; set; }
        public List<T> Inserted { get; set; }
        public List<U> Deleted { get; set; }
        public string KeyName { get; set; }   
    }
    public class ChildUpdater<T, U> : Updater<T, U>
    {
        public List<int> FatherReferences { get; set; }
        public void ImportExternals<L, S>(L[] externalKeys, Expression<Func<T,S>> expression)
        {
            if (externalKeys == null) throw (new ArgumentNullException("externalKeys"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            PropertyInfo property = visitMemeberAccess(expression);
            if (Inserted != null)
            {
                for (int i = 0; i < Inserted.Count; i++)
                {
                    if (FatherReferences[i] >= 0) property.SetValue(Inserted[i], externalKeys[FatherReferences[i]], new object[0]);
                }
            }
        }
        private PropertyInfo visitMemeberAccess(LambdaExpression l)
        {
            Expression e = l.Body;
            PropertyInfo property = null;
            while (e != null && e.NodeType != ExpressionType.Parameter)
            {
                MemberExpression access = e as MemberExpression;
                if (access == null) throw new ArgumentException("expression");
                property = access.Member as PropertyInfo;
                if (property == null) throw new ArgumentException("expression");
                e = access.Expression;
            }
            if (property == null) throw new ArgumentException("expression");
            return property;
        }
    }
     
}
