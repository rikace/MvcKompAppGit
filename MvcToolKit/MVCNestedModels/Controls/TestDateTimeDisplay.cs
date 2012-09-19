using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MVCControlsToolkit.Core;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Mvc.Html;

namespace MVCNestedModels.Controls
{
    public class TestDateTimeDisplay : IDisplayModel
    {

        
        [Range(1000, 3000, ErrorMessage = "wrong year")]
        
        public Nullable<int> Year { get; set; }

        [Range(1, 12, ErrorMessage = "wrong month")]
        public Nullable<int> Month { get; set; }

        [Range(1, 31, ErrorMessage = "wrong day")]
        public Nullable<int> Day { get; set; }

        public object ExportToModel(Type targetType, params object[] context)
        {
            if (Year == 0 && Month == 0 && Day == 0) return null;
            if (!Year.HasValue && !Month.HasValue && !Day.HasValue) return null;
            try
            {
                return new DateTime(Year.Value, Month.Value, Day.Value);
            }
            catch (Exception ex)
            {
                throw (new Exception(" {0} has an incorrect date format", ex));
                
            }
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Nullable<DateTime> date = model as Nullable<DateTime>;
            if (date.HasValue && date.Value != DateTime.MinValue)
            {
                Year = date.Value.Year;
                Month = date.Value.Month;
                Day = date.Value.Day;
            }

        }
    }
    public static class TestDateTimeDisplayHelper
    {
        public static MvcHtmlString SplittDateTimeFor<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, Nullable<DateTime>>> expression, bool useTemplate=false, string templateName=null)
        {

            var fullPropertyPath =
                    htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText(expression));

            if (expression == null) throw (new ArgumentNullException("expression"));
            
            Nullable<DateTime> date=null;
            try
            {
                date = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            string Year;
            string Month;
            string Day;

            if (!date.HasValue || date.Value!=default(DateTime))
            {
                Year = string.Empty;
                Month = string.Empty;
                Day = string.Empty;
            }
            else
            {
                Year = date.Value.Year.ToString();
                Month = date.Value.Month.ToString();
                Day = date.Value.Day.ToString();
            }
            TestDateTimeDisplay displayModel = new TestDateTimeDisplay();
            displayModel.ImportFromModel(date, null);
            
            if (useTemplate)
            {
                if(templateName==null) templateName=typeof(TestDateTimeDisplay).Name;
                ViewDataDictionary<TestDateTimeDisplay> dataDictionary=new ViewDataDictionary<TestDateTimeDisplay>(displayModel);
                dataDictionary.TemplateInfo.HtmlFieldPrefix=fullPropertyPath+".$";
                string res =
                    BasicHtmlHelper.RenderDisplayInfo(htmlHelper, typeof(TestDateTimeDisplay), fullPropertyPath) +
                    htmlHelper.Partial(templateName, dataDictionary);
                return MvcHtmlString.Create(res);
            }
            else
            {
                string res =
                    BasicHtmlHelper.RenderDisplayInfo(htmlHelper, typeof(TestDateTimeDisplay), fullPropertyPath)+
                    htmlHelper.TextBox(fullPropertyPath + ".$.Year", Year, new { style = "text-align:right" }).ToString() + htmlHelper.ValidationMessage(fullPropertyPath + ".Year", "*") + "&nbsp;" +
                    htmlHelper.TextBox(fullPropertyPath + ".$.Month", Month, new { style = "text-align:right" }).ToString() + htmlHelper.ValidationMessage(fullPropertyPath + ".Month", "*") + "&nbsp;" +
                    htmlHelper.TextBox(fullPropertyPath + ".$.Day", Day, new { style = "text-align:right" }).ToString() + htmlHelper.ValidationMessage(fullPropertyPath + ".Day", "*") + "&nbsp;";

                return MvcHtmlString.Create(res);
            }
        }
    }
}