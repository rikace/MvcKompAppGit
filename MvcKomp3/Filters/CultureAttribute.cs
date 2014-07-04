using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    /// <summary>
    /// Action filter to specify culture for controller methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class CultureAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Name of the cookie entry where the culture setting is stored.
        /// </summary>
        private const String CookieLangEntry = "lang";

        /// <summary>
        /// Culture to set
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets the name of the cookie to create to persist the language preference.
        /// </summary>
        public static String CookieName 
        { 
            get { return "_LangPref"; }
        }

        /// <summary>
        /// Applies the required culture before each method executes.
        /// </summary>
        /// <param name="filterContext">Execution context</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = Name; 
            if (String.IsNullOrEmpty(culture))
            {
                culture = GetSavedCultureOrDefault(filterContext.RequestContext.HttpContext.Request);
            }

            // Set culture on current thread
            SetCultureOnThread(culture);

            // Proceed as usual
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Externally callable method that saves the specified culture to a cookie
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="language">Culture to save (it or it-IT)</param>
        /// <param name="expireDays">Expiration in days of the cookie</param>
        public static void SavePreferredCulture(HttpResponseBase response, String language, Int32 expireDays=1)
        {
            var cookie = new HttpCookie(CookieName) { Expires = DateTime.Now.AddDays(expireDays) };
            cookie.Values[CookieLangEntry] = language;
            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Externally callable method that reads the saved culture from the cookie.
        /// </summary>
        /// <param name="httpRequestBase">Response object</param>
        /// <param name="defaultCulture">Default culture</param>
        /// <returns>Saved language</returns>
        public static String GetSavedCultureOrDefault(HttpRequestBase httpRequestBase, String defaultCulture="")
        {
            var culture = defaultCulture;
            var cookie = httpRequestBase.Cookies[CookieName];
            if (cookie != null)
                culture = cookie.Values[CookieLangEntry];
            return culture;
        }

        /// <summary>
        /// Set the specified culture on the current thread
        /// </summary>
        /// <param name="language">Language to set (it or it-IT format)</param>
        private static void SetCultureOnThread(String language)
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}