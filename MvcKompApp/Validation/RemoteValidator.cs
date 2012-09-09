using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcKompApp.Validation
{
    public class RemoteValidator : DataAnnotationsModelValidator<RemoteUID_Attribute>
    {

        public RemoteValidator(ModelMetadata metadata, ControllerContext context,
            RemoteUID_Attribute attribute) :
            base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule()
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "remoteval"
            };

            rule.ValidationParameters["url"] = GetUrl();
            rule.ValidationParameters["parametername"] = Attribute.ParameterName;
            return new ModelClientValidationRule[] { rule };
        }

        private string GetUrl()
        {
            RouteValueDictionary rvd = new RouteValueDictionary() {
            { "controller", Attribute.Controller },
            { "action", Attribute.Action }
        };

            var virtualPath = RouteTable.Routes.GetVirtualPath(ControllerContext.RequestContext,
                Attribute.RouteName, rvd);
            if (virtualPath == null)
            {
                throw new InvalidOperationException("No route matched!");
            }

            return virtualPath.VirtualPath;
        }
    }

    public sealed class RemoteUID_Attribute : ValidationAttribute
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string ParameterName { get; set; }
        public string RouteName { get; set; }

        public override bool IsValid(object value)
        {
            return true;
        }
    } 
}