using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.DataAnnotations
{
    public class DynamicRangeAdapter: DataAnnotationsModelValidator<DynamicRangeAttribute> 
    {
        public DynamicRangeAdapter(ModelMetadata metadata, ControllerContext context, DynamicRangeAttribute attribute)
            : base(metadata, context, attribute) {
        }
        
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            string errorMessage = ErrorMessage;
            object father = ControllerContext.Controller.ViewData.Model;
            ViewContext vc = ControllerContext as ViewContext;
            if (vc != null)
            {
                father = vc.ViewData.Model;
            }
            
            string clientMaximum = null;
            string clientMinimum = null;

            object minDelay = null;
            object maxDelay = null;
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = Metadata.ModelType == typeof(DateTime) || Metadata.ModelType == typeof(DateTime?) ? "daterange" : "dynamicrange"
            };
            object globalMin=Attribute.GetGlobalMinimum(father, this.ControllerContext.Controller.ViewData.Model, out clientMinimum, out minDelay);
            object globalMax=Attribute.GetGlobalMaximum(father, this.ControllerContext.Controller.ViewData.Model, out clientMaximum, out maxDelay);
            if (Metadata.ModelType == typeof(DateTime) || Metadata.ModelType == typeof(DateTime?))
            {
                bool isClientBlock = vc.ViewData["ClientBindings"] != null
                    || vc.HttpContext.Items.Contains("ClientTemplateOn");
                if (!isClientBlock)
                {
                    if (globalMin != null)
                    {
                        
                        globalMin = BasicHtmlHelper.ClientValidationDate(globalMin, isClientBlock);
                    }
                    if (globalMax != null)
                    {
                        globalMax = BasicHtmlHelper.ClientValidationDate(globalMax, isClientBlock);
                    }
                }
            }
            rule.ValidationParameters.Add("min", globalMin);
            rule.ValidationParameters.Add("max", globalMax);

            if (clientMaximum != null || clientMinimum != null)
            {
                var drule = new ModelClientValidationRule
                {
                    ErrorMessage = errorMessage,
                    ValidationType = Metadata.ModelType == typeof(DateTime) || Metadata.ModelType == typeof(DateTime?) ? "clientdynamicdaterange" : "clientdynamirange"
                };
                drule.ValidationParameters.Add("min", clientMinimum);
                drule.ValidationParameters.Add("max", clientMaximum);
                drule.ValidationParameters.Add("mindelay", minDelay);
                drule.ValidationParameters.Add("maxdelay", maxDelay);
                return new[] { rule, drule };
            }
            else
            {
                return new[] { rule };
            }
        }
    }
}
