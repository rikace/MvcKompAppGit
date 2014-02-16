using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MvcNotes.Utils
{
    public static class Find
    {
        /// <summary>
        /// A bit verbose, but since we're using dynamics you can't attach an extension method - which is sad.
        /// This includes core system LINQ stuff, like "FirstOrDefault".
        /// </summary>
        static string FindMatch(string source, string find)
        {
            Regex reg = new Regex(find, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            return reg.Match(source).Value;

        }
        /// <summary>
        /// Matching core for loading up stuff on a web page
        /// </summary>
        static List<string> FindMatches(string source, string find)
        {
            Regex reg = new Regex(find, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            List<string> result = new List<string>();

            foreach (Match m in reg.Matches(source))
                result.Add(m.Value);
            return result;
        }

        /// <summary>
        /// This method parses the Get request into digestable little chunks so you can query the response's title, links,
        /// headings, body, body links, etc.
        /// </summary>
        static void ParseResult(dynamic result)
        {
            string bodyPattern = "(?<=<body>).*?(?=</body>)";
            string titlePattern = "(?<=<title>).*?(?=</title>)";
            string linkPattern = "<a.*?>.*?</a>";
            string headPattern = @"(?<=<h\d\w*?>).*?(?=</h\d>)";

            //have to cast this ToString() as result.Html is a dynamic (which is actually an HtmlString)
            var html = result.Html.ToString();

            result.Title = FindMatch(html, titlePattern);
            result.Links = FindMatches(html, linkPattern);
            result.Headings = FindMatches(html, headPattern);
            result.Body = FindMatch(html, bodyPattern);
            result.BodyLinks = FindMatches(result.Body.ToString(), linkPattern);
        }
    }
}