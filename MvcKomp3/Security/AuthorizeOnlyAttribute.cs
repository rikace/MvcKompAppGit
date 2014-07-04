using System;
using System.Web.Mvc;

namespace BookSamples.Components.Security
{
    /// <summary>
    /// This attribute enables only authenticated users with the specified name/role.
    /// </summary>
    public class AuthorizedOnlyAttribute : AuthorizeAttribute
    {
        public AuthorizedOnlyAttribute()
        {
            View = "401";
            Master = String.Empty;
        }

        public String View { get; set; }
        public String Master { get; set; }

        /// <summary>
        /// Overrides the base method to add an additional check.
        /// </summary>
        /// <param name="filterContext">HTTP context</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            CheckIfUserIsAuthenticated(filterContext);
        }

        /// <summary>
        /// Redirects to the specified error view logged users with insufficient privileges.
        /// </summary>
        /// <param name="filterContext">HTTP context</param>
        private void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
        {
            // If Result is null, we’re OK
            if (filterContext.Result == null)
                return;

            // Is this an Ajax request?
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For an Ajax request, just end the request 
                filterContext.HttpContext.Response.StatusCode = 401; // must maintain this line, if you're ending the request
                filterContext.HttpContext.Response.End();
            }

            // If here, you’re getting an HTTP 401 status code
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = new ViewResult { ViewName = View, MasterName = Master };
                filterContext.Result = result;
            }
        }
    }
}