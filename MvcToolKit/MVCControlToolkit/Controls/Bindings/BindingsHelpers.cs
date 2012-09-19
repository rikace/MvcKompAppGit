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
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;


namespace MVCControlsToolkit.Controls.Bindings
{
    
    
    public static class BindingsHelpers
    {
        private static string modelScript =
            @"
            <script language='javascript' type='text/javascript'>
                {0} = ko.mapping.fromJS({2}); 
                MvcControlsToolkit_ClientViewModel_Init({1}, '{5}', '{6}');
                $(document).ready(function()
                {{
                    {3} 
                }});
            </script>
            ";
        private static string modelScriptNoDep =
            @"
            <script language='javascript' type='text/javascript'>
                {0} = {2}; 
                MvcControlsToolkit_ClientViewModel_Init({1}, '{5}', '{6}');
                $(document).ready(function()
                {{
                    {3} 
                }});
            </script>
            ";
        private static string rootBingingScript =
            @"
            <script language='javascript' type='text/javascript'>
                {0} = ko.mapping.fromJS({2}); 
                MvcControlsToolkit_ClientViewModel_Init({1}, '{5}', '{6}');
                $(document).ready(function()
                {{
                    {3} 
                    ko.applyBindings({1}); 
                }});
            </script>
            ";
        private static string elementBingingScript =
            @"
            <script language='javascript' type='text/javascript'>
                {0} = ko.mapping.fromJS({2}); 
                MvcControlsToolkit_ClientViewModel_Init({1}, '{5}', '{6}');
                $(document).ready(function()
                {{
                    {3} 
                    ko.applyBindings({1}, document.getElementById('{4}')); 
                }});
            </script>
            ";
        public static IBindingsBuilder<T> ItemClientViewModel<T>(
            this HtmlHelper<T> htmlHelper
            )
            where T : class, new()
        {
            
            string validationType = null;

            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            return new BindingsBuilder<T>(htmlHelper.ViewContext.Writer, string.Empty, string.Empty, validationType, null, htmlHelper);
        }
        public static IBindingsBuilder<T> SAClientViewModel<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            string uniqueName,
            T model,
            string partialPrefix = null,
            string htmlElementId = null,
            bool initialSave = true,
            bool applyBindings = true,
            bool applyDependencies = true
            )
            where T : class, new()
        {
            return htmlHelper.InnerClientViewModel(
                uniqueName,
                model,
                partialPrefix,
                partialPrefix,
                htmlElementId,
                initialSave,
                applyBindings,
                applyDependencies);
        }
        internal static IBindingsBuilder<T> InnerClientViewModel<T>(
            this HtmlHelper htmlHelper,
            string uniqueName,
            T model,
            string partialPrefix,
            string prefix,
            string htmlElementId,
            bool initialSave,
            bool applyBindings,
            bool applyDependencies=true
            )
            where T : class, new()
        {
            if (partialPrefix == null) partialPrefix = string.Empty;
            if (prefix == null) prefix = string.Empty;
            if (uniqueName == null) throw (new ArgumentNullException("uniqueName")); 
            
            string validationType = null;

            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            if (model == null)
                return new BindingsBuilder<T>(null, uniqueName, prefix, validationType, null, htmlHelper);

            string script = null;
            if (applyBindings)
            {
                if (htmlElementId == null) script = rootBingingScript;
                else script = elementBingingScript;
            }
            else if (applyDependencies)
            {
                script = modelScript;
            }
            else
            {
                script = modelScriptNoDep;
            }

            ModelTranslator<T> translator = new ModelTranslator<T>();
            translator.ImportFromModel(model);
            
            htmlHelper.ViewContext.Writer.Write(
                BasicHtmlHelper.RenderDisplayInfo(htmlHelper, 
                typeof(ModelTranslator<T>),
                partialPrefix, true));
            object ocount = htmlHelper.ViewContext.Controller.ViewData["__ClientModelsCount__"];
            int count = 0;
            if (ocount != null) count = (int)ocount;
            count++;
            htmlHelper.ViewContext.Controller.ViewData["__ClientModelsCount__"] = count;

            string jsonHiddenId = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(prefix, "$.JSonModel"+count.ToString()));
            
            htmlHelper.ViewContext.Writer.Write(
                BasicHtmlHelper.SafeHiddenUC(htmlHelper,
                    BasicHtmlHelper.AddField(partialPrefix, 
                    "$.JSonModel"),
                    string.Empty,
                    jsonHiddenId
                    )
                );
            string saveScript = string.Empty;
            if (initialSave) saveScript = uniqueName + ".save();";
            string assignement = uniqueName;
            if (!assignement.Contains('.')) assignement = "var " + assignement;
            string jsonModel = string.Format(script, assignement, uniqueName, translator.JSonModel, saveScript, htmlElementId, jsonHiddenId, validationType);
            htmlHelper.ViewContext.Writer.Write(jsonModel );

            
            IBindingsBuilder<T> result =
                new BindingsBuilder<T>(htmlHelper.ViewContext.Writer, uniqueName, prefix, validationType, jsonHiddenId, htmlHelper);
     /*       result.AddMethod("save", string.Format(@"
                    function(){{
                            document.getElementById('{0}').value = ko.mapping.toJSON(this);
                    }}",
                       jsonHiddenId));
            result.AddMethod("validateAndSave", string.Format(@"
                    function(){{
                        if(MvcControlsToolkit_FormIsValid('{0}', '{1}')){{
                            document.getElementById('{0}').value = ko.mapping.toJSON(this);
                            return true;
                        }}
                        return false;
                    }}",
                       jsonHiddenId, validationType));
            result.AddMethod("saveAndSubmit", string.Format(@"
                    function(){{
                        if(this.validateAndSave()){{
                            $('#{0}').parents('form').submit();
                        }}
                    }}",
                       jsonHiddenId
                       ));
            result.AddMethod("saveAndSubmitAlone", string.Format(@"
                    function(formId){{
                        if(MvcControlsToolkit_FormIsValid(formId, '{1}')){{
                            this.save();
                            $('#{0}').parents('form').submit();
                        }}
                    }}",
                       jsonHiddenId,
                       validationType
                       )); */
            return result;
        
        }
        public static IBindingsBuilder<T> ClientViewModel<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            string uniqueName,
            Expression<Func<VM, T>> expression,
            string htmlElementId = null,
            bool initialSave = true,
            bool applyBindings = true,
            bool applyDependencies = true
            )
            where T : class, new()
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string partialPrefix = ExpressionHelper.GetExpressionText(expression);
            string prefix=htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
            T model = null;
            try
            {
                model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }

            return InnerClientViewModel(htmlHelper,
                uniqueName,
                model,
                partialPrefix,
                prefix,
                htmlElementId,
                initialSave,
                applyBindings,
                applyDependencies);

        }

        public static IBindingsBuilder<T> ClientViewModel<VM, M, T>(
            this HtmlHelper<VM> htmlHelper,
            string uniqueName,
            M baseModel,
            Expression<Func<M, T>> expression,
            string htmlElementId = null,
            bool initialSave = true,
            bool applyBindings = true,
            bool applyDependencies = true
            )
            where T : class, new()
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (baseModel == null) throw (new ArgumentNullException("baseModel"));
            string partialPrefix = ExpressionHelper.GetExpressionText(expression);
            T model = null;
            try
            {
                model = expression.Compile().Invoke(baseModel);
            }
            catch
            {
            }

            return htmlHelper.InnerClientViewModel(
                uniqueName,
                model,
                partialPrefix,
                partialPrefix,
                htmlElementId,
                initialSave,
                applyBindings,
                applyDependencies);

        }    


    }
}
