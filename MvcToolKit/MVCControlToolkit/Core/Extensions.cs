using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;

namespace MVCControlsToolkit.Core
{
    public static class Extensions
    {
        public static void Register(Type themedControlsResources=null, string defaultTheme="Standard")
        {
            ModelBinders.Binders.DefaultBinder = new MVCControlsToolkit.Core.ExDefaultBinder();
            ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProviderExt();
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new DataAnnotationsModelValidatorProviderExt());
            ModelValidatorProviders.Providers.Add(new DataErrorInfoModelValidatorProvider());
            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProviderExt());
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(MVCControlsToolkit.DataAnnotations.RangeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DynamicRangeAttribute), typeof(DynamicRangeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DateRangeAttribute), typeof(DateRangeAdapter));
            MVCControlsToolkit.Controls.ThemedControlsStrings.ResourceType = themedControlsResources;
            MVCControlsToolkit.Controls.ThemedControlsStrings.DefaultTheme = defaultTheme;
        }
    }
}
