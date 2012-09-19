using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace MVCControlsToolkit.Core
{
    public class ClientDataTypeModelValidatorProviderExt : ClientDataTypeModelValidatorProvider
    {
        public static Type ErrorMessageResources { get; set; }
        public static string NumericErrorKey { get; set; }
        public static string DateTimeErrorKey { get; set; }
        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>(new Type[] {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        });
        private static bool IsNumericType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type); // strip off the Nullable<>
            return _numericTypes.Contains(underlyingType ?? type);
        }
        internal sealed class NumericModelValidator : ModelValidator
        {
            public NumericModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
                : base(metadata, controllerContext)
            {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
            {
                ModelClientValidationRule rule = new ModelClientValidationRule()
                {
                    ValidationType = "number",
                    ErrorMessage = MakeErrorString(Metadata.GetDisplayName())
                };

                return new ModelClientValidationRule[] { rule };
            }

            private static string MakeErrorString(string displayName)
            {
                // use CurrentCulture since this message is intended for the site visitor
                return String.Format(CultureInfo.CurrentCulture, ErrorMessageResources.GetProperty(NumericErrorKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string, displayName);
            }

            public override IEnumerable<ModelValidationResult> Validate(object container)
            {
                // this is not a server-side validator
                return Enumerable.Empty<ModelValidationResult>();
            }
        }
        internal sealed class DateTimeModelValidator : ModelValidator
        {
            public DateTimeModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
                : base(metadata, controllerContext)
            {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
            {
                ModelClientValidationRule rule = new ModelClientValidationRule()
                {
                    ValidationType = "globalizeddate",
                    ErrorMessage = MakeErrorString(Metadata.GetDisplayName())
                };

                return new ModelClientValidationRule[] { rule };
            }

            private static string MakeErrorString(string displayName)
            {
                // use CurrentCulture since this message is intended for the site visitor
                return String.Format(CultureInfo.CurrentCulture, ErrorMessageResources.GetProperty(DateTimeErrorKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string, displayName);
            }

            public override IEnumerable<ModelValidationResult> Validate(object container)
            {
                // this is not a server-side validator
                return Enumerable.Empty<ModelValidationResult>();
            }
        }
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            List<ModelValidator> res = null;
            if (NumericErrorKey == null || ErrorMessageResources == null)
                res = base.GetValidators(metadata, context).ToList();
            else
            {
                res = new List<ModelValidator>();
                if (IsNumericType(metadata.ModelType))
                {
                    res.Add(new NumericModelValidator(metadata, context));
                }
            }
            if ( (metadata.ModelType == typeof(DateTime) || metadata.ModelType == typeof(DateTime?)))
            {
                if(ErrorMessageResources != null && DateTimeErrorKey != null)
                    res.Add(new DateTimeModelValidator(metadata, context));
            }
            return res;
        }
    }
}
