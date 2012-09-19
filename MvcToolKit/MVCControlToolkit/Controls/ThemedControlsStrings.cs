/* ****************************************************************************
 *
 * Copyright (c) Francesco Abbruzzese. All rights reserved.
 * francesco@dotnet-programming.com
 * http://www.dotnet-programming.com/
 * 
 * This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
 * and included in the license.txt file of this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;

namespace MVCControlsToolkit.Controls
{
    public static class ThemedControlsStrings
    {
        public static Type ResourceType{get; set;}
        public static string DefaultTheme;
        public static string GetTheme()
        {
            if (HttpContext.Current.Items.Contains("MvcControlsToolkitTheme"))
            {
                return HttpContext.Current.Items["MvcControlsToolkitTheme"] as string;
            }
            else
            {
                HttpContext context = HttpContext.Current;
                HttpCookie cookie = context.Request.Cookies["_theme"];
                if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value) && cookie.Value.Length <= 100 && new Regex(@"^\w+$").IsMatch(cookie.Value))
                {
                    HttpContext.Current.Items["MvcControlsToolkitTheme"] = cookie.Value;
                    return cookie.Value;
                }
                else
                    return DefaultTheme;
            }
        }
        public static void SetTheme(string theme, bool permanent=false)
        {
            if (!string.IsNullOrWhiteSpace(theme) && theme.Length <= 100 && new Regex(@"^\w+$").IsMatch(theme))
            {
                HttpContext.Current.Items["MvcControlsToolkitTheme"] = theme;
                if (permanent)
                {
                    HttpContext context = HttpContext.Current;
                    HttpCookie cookie = context.Request.Cookies["_theme"];
                    if (cookie != null)
                        cookie.Value = theme;   // update cookie value
                    else
                    {

                        cookie = new HttpCookie("_theme");
                        cookie.HttpOnly = false; // Not accessible by JS.
                        cookie.Value = theme;
                        cookie.Expires = DateTime.Now.AddYears(1);
                    }
                    context.Response.Cookies.Add(cookie);


                }
            }
        }
        public static string Get(string key, string name)
        {
            if (ResourceType == null) ResourceType = typeof(ControlsResources);
            string result = null;
            try
            {
                PropertyInfo prop = ResourceType.GetProperty(key+"_"+name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop == null)
                    prop = ResourceType.GetProperty(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop==null) 
                    prop = typeof(ControlsResources).GetProperty(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null) result = prop.GetValue(null, null) as string;
                
            }
            catch
            {
            }
            return result;
        }
    }
}
