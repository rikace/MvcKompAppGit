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
using System.Collections;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;
using System.Web.Script.Serialization;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace MVCControlsToolkit.Controls.Bindings
{
    internal class ModelTranslatorBase: IUpdateModelState
    {
        protected string prefix;
        protected System.Web.Mvc.ModelStateDictionary modelState;
        public string JSonModel { get; set; }
        protected void Validate(object x, object father, string propertyName, string currPrefix)
        {
            if (x == null) return;
            if (x is IConvertible)
            {
                if (father == null) return;
                ValidationContext ctx = new ValidationContext(father, null, null);
                ctx.MemberName = propertyName;
                List<ValidationResult> errors = new List<ValidationResult>();
                bool success = Validator.TryValidateProperty(x, ctx, errors);
                if (!success)
                {
                    foreach (ValidationResult vs in errors)
                    {
                        modelState.AddModelError(currPrefix, vs.ErrorMessage);
                    }
                }

            }
            else if (x is IEnumerable)
            {
                int i = 0;
                foreach (object y in x as IEnumerable)
                {
                    Validate(y, null, null,
                        currPrefix +
                        string.Format("[{0}]", i)
                        );
                    i++;
                }
            }
            else
            {
                Type currType = x.GetType();
                if (currType.IsClass)
                {
                    List<ValidationResult> errors = new List<ValidationResult>();
                    bool success = Validator.TryValidateObject(x, new ValidationContext(x, null, null), errors, true);
                    ValidationAttribute[] attrs = currType.GetCustomAttributes(typeof(ValidationAttribute), true) as ValidationAttribute[];

                    foreach (ValidationAttribute attr in attrs)
                    {
                        ValidationResult vs = attr.GetValidationResult(x, new ValidationContext(x, null, null));
                        if (vs != null && vs != ValidationResult.Success)
                            modelState.AddModelError(currPrefix, vs.ErrorMessage);
                    }

                    foreach (PropertyInfo prop in currType.GetProperties())
                    {

                        if ((prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)) && prop.CanRead && prop.CanWrite)
                        {
                            object dateVal = prop.GetValue(x, new object[0]);
                            if (dateVal != null)
                            {
                                prop.SetValue(x, ((DateTime)dateVal).ToLocalTime(), new object[0]);
                            }

                        }
                        Validate(prop.GetValue(x, new object[0]),
                            x,
                            prop.Name,
                            BasicHtmlHelper.AddField(currPrefix, prop.Name));
                    }
                }
            }
        }
        public void GetCurrState(string currPrefix, int updateIndex, System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (currPrefix == "display") currPrefix = string.Empty;
            this.prefix = currPrefix;
            this.modelState = modelState;
        }

        public bool MoveState
        {
            get { return false; }
        }
    }
    internal class SipleModelTranslator<T> : ModelTranslatorBase, IDisplayModel
    {


        public object ExportToModel(Type targetType, params object[] context)
        {
            if (string.IsNullOrWhiteSpace(JSonModel)) return null;
            if (!targetType.IsAssignableFrom(typeof(T)))
                throw (new NotSupportedException(string.Format(ControlsResources.NotCompatibleTypes, typeof(T).FullName, targetType.FullName)));
            object result = BasicHtmlHelper.ClientDecode(JSonModel, typeof(T));
            Validate(result, null, null, prefix);
            return result;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            JSonModel = BasicHtmlHelper.ClientEncode(model);

        }

    }
    internal class ModelTranslator<T> : ModelTranslatorBase, IDisplayModel
    {
        private static Regex dateRewrite;
        private static Regex dateRewriteOut;
        static ModelTranslator()
        {
            dateRewrite=new Regex("\""+@"\\/Date\(((-)?\d+)(?:[-+]\d+)?\)\\/"+"\"");
            dateRewriteOut = new Regex("\"" + @"\\\\/Date\(((-)?\d+)(?:[-+]\d+)?\)\\\\/" + "\"");
        }
        
        
        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (string.IsNullOrWhiteSpace(JSonModel)) return null;
            if (!TargetType.IsAssignableFrom(typeof(T)))
                throw (new NotSupportedException(string.Format(ControlsResources.NotCompatibleTypes, typeof(T).FullName, TargetType.FullName)));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //JSonModel = dateRewriteOut.Replace(JSonModel, "\"\\/Date($1)\\/\"");
            T result = serializer.Deserialize<T>(JSonModel);
            Validate(result, null, null, prefix);
            return result;
        }
        
        public void ImportFromModel(object model, params object[] context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            JSonModel = serializer.Serialize(model) ;
            JSonModel = dateRewrite.Replace(JSonModel, "new Date($1)");
        }

        
    }
    internal class JSONAdapter : ModelTranslatorBase, IDisplayModel
    {
        private static Regex dateRewrite;
        private static Regex dateRewriteOut;
        static JSONAdapter()
        {
            dateRewrite = new Regex("\"" + @"\\/Date\(((-)?\d+)(?:[-+]\d+)?\)\\/" + "\"");
            dateRewriteOut = new Regex("\"" + @"\\\\/Date\(((-)?\d+)(?:[-+]\d+)?\)\\\\/" + "\"");
        }


        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (string.IsNullOrWhiteSpace(JSonModel)) return null;
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            JSonModel = dateRewriteOut.Replace(JSonModel, "\"\\/Date($1)\\/\"");
            object result = serializer.Deserialize(JSonModel, TargetType);
            Validate(result, null, null, prefix);
            return result;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            JSonModel = serializer.Serialize(model);
            JSonModel = dateRewrite.Replace(JSonModel, "new Date($1)");
        }


    }
    
    internal class ModelTranslatorLight : IDisplayModel
    {
        
        static ModelTranslatorLight()
        {
            
        }
        public string JSonModel { get; set; }
        public object ExportToModel(Type targetType, params object[] context)
        {
            return BasicHtmlHelper.ClientDecode(JSonModel, targetType);
        }
        
        public void ImportFromModel(object model, params object[] context)
        {
            JSonModel = BasicHtmlHelper.ClientEncode(model);
            
        }

        
    }
}
