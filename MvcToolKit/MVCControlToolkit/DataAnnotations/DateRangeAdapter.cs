using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.DataAnnotations
{
    public class DateRangeAdapter: DataAnnotationsModelValidator<DateRangeAttribute> 
    {
        public DateRangeAdapter(ModelMetadata metadata, ControllerContext context, DateRangeAttribute attribute)
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
           
            object globalMin = Attribute.GetGlobalMinimum(father, this.ControllerContext.Controller.ViewData.Model, out clientMinimum, out minDelay);
            object globalMax = Attribute.GetGlobalMaximum(father, this.ControllerContext.Controller.ViewData.Model, out clientMaximum, out maxDelay);
            List<ModelClientValidationRule> res = new List<ModelClientValidationRule>();
            if (globalMin != null || globalMax != null)
            {
                bool isClientBlock = vc.ViewData["ClientBindings"] != null
                    || vc.HttpContext.Items.Contains("ClientTemplateOn");
                if (!isClientBlock)
                {
                    if (globalMin != null) {
                        globalMin = BasicHtmlHelper.ClientValidationDate(globalMin, isClientBlock);
                    }
                    if (globalMax != null)
                    {
                        globalMax = BasicHtmlHelper.ClientValidationDate(globalMax, isClientBlock);
                    }
                }
                var rule = new ModelClientValidationRule
                {
                    ErrorMessage = errorMessage,
                    ValidationType = "daterange"
                };

                rule.ValidationParameters.Add("min", globalMin );
                rule.ValidationParameters.Add("max", globalMax);
                res.Add(rule);
            }

            if (clientMaximum != null || clientMinimum != null)
            {
                var drule = new ModelClientValidationRule
                {
                    ErrorMessage = errorMessage,
                    ValidationType = "clientdynamicdaterange"
                };
                drule.ValidationParameters.Add("min", clientMinimum);
                drule.ValidationParameters.Add("max", clientMaximum);
                drule.ValidationParameters.Add("mindelay", minDelay);
                drule.ValidationParameters.Add("maxdelay", maxDelay);
                res.Add(drule);
            }
            return res.ToArray();
        }
    }
}
