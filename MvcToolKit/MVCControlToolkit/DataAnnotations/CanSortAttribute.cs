using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.Core;
using System.Reflection;
using System.Web.Mvc;

namespace MVCControlsToolkit.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class CanSortAttribute: Attribute
    {
        public FilterCondition Allow { get; set; }
        public FilterCondition Deny { get; set; }

        public CanSortAttribute()
        {
            Allow = FilterCondition.Equal |
                FilterCondition.LessThan |
                FilterCondition.LessThanOrEqual |
                FilterCondition.GreaterThan |
                FilterCondition.GreaterThanOrEqual |
                FilterCondition.NotEqual |
                FilterCondition.StartsWith |
                FilterCondition.EndsWith |
                FilterCondition.IsContainedIn |
                FilterCondition.Contains;
            Deny = FilterCondition.IsContainedIn |
                FilterCondition.Contains;
        }
        public bool Allowed(FilterCondition condition)
        {
            
                return (condition & Allow & (~ Deny)) == condition;
            
        }
        public FilterCondition Filter (FilterCondition conditions)
        {
            return conditions & Allow & (~Deny);
        }
        public static FilterCondition AllowedForProperty(FilterCondition conditions, Type type, string propertyName)
        {
            if (type == null) throw new ArgumentException("type");
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("propertyName");
           
            PropertyAccessor pa = null;
            try
            {
                pa = new PropertyAccessor(propertyName, type);
            }
            catch { }
            if (pa == null) throw new ArgumentException("propertyName");
            type =  pa.Property.PropertyType;
            CanSortAttribute[] attributes = pa[typeof(CanSortAttribute)] as CanSortAttribute[];
            if (attributes.Length == 0) return FilterCondition.None;
            if (type != typeof(string))
            {
                conditions = conditions & (~(FilterCondition.StartsWith |
                                             FilterCondition.EndsWith));
            }
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                conditions = conditions & (~FilterCondition.Contains);
            }
            return attributes[0].Filter(conditions);
        }
        public static FilterCondition AllowedForProperty(Type type, string propertyName)
        {
            if (type == null) throw new ArgumentException("type");
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("propertyName");

            PropertyAccessor pa = null;
            try
            {
                pa = new PropertyAccessor(propertyName, type);
            }
            catch { }
            if (pa == null) throw new ArgumentException("propertyName");
            type = pa.Property.PropertyType;
            CanSortAttribute[] attributes = pa[typeof(CanSortAttribute)] as CanSortAttribute[];
            if (attributes.Length == 0) return FilterCondition.None;
            
            FilterCondition conditions = attributes[0].Allow & (~attributes[0].Deny);
            if (type != typeof(string))
            {
                conditions = conditions & (~(FilterCondition.StartsWith |
                                             FilterCondition.EndsWith));
            }
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                conditions = conditions & (~FilterCondition.Contains);
            }
            return conditions;
        }
        public static FilterCondition AllowedForProperty<T, F>(Expression<Func<T, F>> field)
        {
            if (field == null) throw new ArgumentException("field");
            return AllowedForProperty(typeof(T), ExpressionHelper.GetExpressionText(field));
        }
        public static FilterCondition AllowedForProperty<T, F>(FilterCondition conditions, Expression<Func<T, F>> field)
        {
            if (field == null) throw new ArgumentException("field");
            return AllowedForProperty(conditions, typeof(T), ExpressionHelper.GetExpressionText(field));
        }
    }
}
