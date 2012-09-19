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
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCControlsToolkit.DataAnnotations
{
   [AttributeUsage(AttributeTargets.Property)]
    public class MandatoryAttribute: ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value is bool && !((bool)value)) return false;
            else return true;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage=FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType="mandatory";
            yield return rule;
        }
        
    }
}
