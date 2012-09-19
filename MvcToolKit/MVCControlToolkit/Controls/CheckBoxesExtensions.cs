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
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public static class CheckBoxesExtensions
    {
        private static string mutualExclusiveScript =
        @"
        <script language='javascript' type='text/javascript'> 
            $(document).ready(function() {{
                $('.{0}').click(function() {{
                  var checked =  $(this).attr('checked');
                  if (checked){{
                     $('.{0}').attr('checked', false);
                     $(this).attr('checked', true);
                  }}
                 
                  
                }});
            }});
        </script>
        ";
       

        public static MvcHtmlString MutualExclusiveChoice<VM>(this HtmlHelper<VM> htmlHelper, string cssClass)
        {
            return MvcHtmlString.Create(string.Format(mutualExclusiveScript, cssClass));
        }

       
    }
}
