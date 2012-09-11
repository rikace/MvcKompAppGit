using System.Web.Mvc;

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
    }
}