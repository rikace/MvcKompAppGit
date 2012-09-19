using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.DataAnnotations;

namespace MVCControlsToolkit.Controls.DataFilter
{
    public class DataFilterClause<T, F>: IUpdateModel, IVisualState, IHandleUpdateIndex
    {

        public FilterCondition Condition { get; set; }
        public F Search { get; set; }
        public bool Selected { get; set; }
        private string modelName;
        private IDictionary store;
        public object UpdateModel(object model, string[] fields)
        {
            
            System.Web.Mvc.ModelState currModelState= ModelState[string.Format("{1}.$${0}.$.Search", Index, modelName)];
            if (currModelState.Errors.Count > 0)
            {
                currModelState.Errors.Clear();
                return model;
            }
            if (!Selected) return model;
            if (fields == null || fields.Length <= 0) return model;
            PropertyAccessor pa = null;
            try
            {
                pa = new PropertyAccessor(fields[0], typeof(T));
            }
            catch { }
            if (pa == null) return model;
            CanSortAttribute[] cs = pa[typeof(CanSortAttribute)] as CanSortAttribute[];
            if (cs == null || cs.Length == 0) return model;
            string key = modelName + "_Filter_" + fields[0] + "_Filter_" + fields[1];
            store[key] = this;
            Expression<Func<T, bool>> previousFilter = model as Expression<Func<T, bool>>;
            FilterBuilder<T> filterBuilder = new FilterBuilder<T>();
            if (previousFilter != null)
                filterBuilder.Add(true, previousFilter);
            filterBuilder.Add(Condition, fields[0], Search);
            return filterBuilder.Get();
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNames, object[] args = null)
        {
            if (model == null) return;
            if (args == null || args.Length<2) return;
            if (fields == null || fields.Length == 0) return;
            store = args[0] as IDictionary;
            if (store == null) return;
            modelName = args[1] as string;
            if (store == null || modelName == null) return;
            string key = modelName + "_Filter_" + fieldNames[0] + "_Filter_" + fieldNames[1];

            if (store.Contains(key))
            {
                DataFilterClause<T, F> previous = store[key] as DataFilterClause<T, F>;
                if (previous != null)
                {
                    this.Search = previous.Search;
                    this.Selected = previous.Selected;
                    this.Condition = previous.Condition;
                }
            }
        }



        public System.Collections.IDictionary Store
        {
            set { store=value; }
        }

        public string ModelName
        {
            set { modelName=value; }
        }

        public int Index
        {
            get;
            set;
        }
        public ModelStateDictionary ModelState { get; set; }
    }

    
}
namespace MVCControlsToolkit.Controls
{
    using MVCControlsToolkit.Controls.DataFilter;

    internal class FilterOptions
    {
        public string FilterName { get; set; }
        public int FilterCode { get; set; }
    } 
    public static class DataFilterClauseHelpers
    {

        public static Type GetFieldType<T, F>(Expression<Func<T, F>> field)
        {
            return typeof(F);
        }
        public static MvcHtmlString FilterClauseSelect<VM, T, F>(
            HtmlHelper<VM> htmlHelper,
            FilterCondition value,
            Expression<Func<T, F>> field,
            IDictionary<string, object> htmlAttributes=null)
        {
            if (field == null) throw new ArgumentNullException("field");
            var items = new List<FilterOptions>();
            items.Add(new FilterOptions {FilterCode=Convert.ToInt32(FilterCondition.Equal), FilterName=ThemedControlsStrings.Get("FilterCondition_Equal", string.Empty)});
            items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.LessThan), FilterName = ThemedControlsStrings.Get("FilterCondition_LessThan", string.Empty) });
            items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.LessThanOrEqual), FilterName = ThemedControlsStrings.Get("FilterCondition_LessThanOrEqual", string.Empty) });
            items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.GreaterThan), FilterName = ThemedControlsStrings.Get("FilterCondition_GreaterThan", string.Empty) });
            items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.GreaterThanOrEqual), FilterName = ThemedControlsStrings.Get("FilterCondition_GreaterThanOrEqual", string.Empty) });
            items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.NotEqual), FilterName = ThemedControlsStrings.Get("FilterCondition_NotEqual", string.Empty) });

            if (typeof(F) == typeof(string))
            {
                items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.StartsWith), FilterName = ThemedControlsStrings.Get("FilterCondition_StartsWith", string.Empty) });
                items.Add(new FilterOptions { FilterCode = Convert.ToInt32(FilterCondition.EndsWith), FilterName = ThemedControlsStrings.Get("FilterCondition_EndsWith", string.Empty) });
            }
            return
                htmlHelper.DropDownList(
                    "Condition",
                    Convert.ToInt32(value),
                    htmlAttributes,
                    ChoiceListHelper.Create(items, m => m.FilterCode, m => m.FilterName)
                    );

        }
        public static HtmlHelper<DataFilterClause<T, F>> DataFilterClauseFor<VM, T, F>
            (this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, Expression<Func<T, bool>>>> filter,
            Expression<Func<T, F>> field,
            string filterName)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            if (field == null) throw new ArgumentNullException("field");
            if (filterName == null) throw new ArgumentNullException("filterName");
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string modelName = ExpressionHelper.GetExpressionText(filter);
            return htmlHelper.RenderWith(
                htmlHelper.InvokeUpdateTransform(
                    filter,
                    new DataFilterClause<T, F>(),
                    new string[] { fieldName, filterName },
                    new object[]
                    {
                        htmlHelper.ViewContext.RequestContext.HttpContext.Items, 
                        modelName
                    },
                    false));

        }
    }
}
