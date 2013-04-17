using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcKompApp.Infrastructure
{
    public static class HtmlHelpers
    {
        public static TagBuilder Image(this HtmlHelper helper, string imageUrl)
        {
            TagBuilder tag = new TagBuilder("img");
            tag.MergeAttribute("src", imageUrl);
            return tag;
        }

        //     <p><%: Html.OptionalText("", "{0}", "N/A") %></p>
        public static MvcHtmlString OptionalText(this HtmlHelper helper, String text,
                String format = "{0}",
                String alternateText = "",
                String alternateFormat = "{0}")
        {
            var actualText = text;
            var actualFormat = format;

            if (String.IsNullOrEmpty(actualText))
            {
                actualText = alternateText;
                actualFormat = alternateFormat;
            }

            return MvcHtmlString.Create(String.Format(actualFormat, actualText));
        }


        public static string Image(this HtmlHelper helper, string id, string url, string alternateText)
        {
            return Image(helper, id, url, alternateText, null);
        }

        public static string Image(this HtmlHelper helper, string id, string url, string alternateText, object htmlAttributes)
        {
            // Create tag builder 
            var builder = new TagBuilder("img");

            // Create valid id 
            builder.GenerateId(id);

            // Add attributes 
            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag 
            return builder.ToString(TagRenderMode.SelfClosing);
        }
    }
}