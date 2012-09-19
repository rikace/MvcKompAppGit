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

namespace MVCControlsToolkit.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormatAttribute : DisplayFormatAttribute
    {
        
        public string Prefix { get; set; }
        public string Postfix { get; set; }
        public string ClientFormat {get; set;}
        public bool ExtractClientFormat { get; set; }
        public FormatAttribute(): base()
        {
        }
        public FormatAttribute(Type resourceType, string prefixKey, string postfixKey, string nullStringKey )
            : base()
        {
            if (resourceType == null) return;
            if (!string.IsNullOrWhiteSpace(prefixKey))
            {
                try
                {
                    Prefix=resourceType.GetProperty(prefixKey, System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static).GetValue(null, null) as string;
                }
                catch
                {
                }
            }
            if (!string.IsNullOrWhiteSpace(postfixKey))
            {
                try
                {
                    Postfix = resourceType.GetProperty(postfixKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string;
                }
                catch
                {
                }
            }
            if (!string.IsNullOrWhiteSpace(nullStringKey))
            {
                try
                {
                    NullDisplayText = resourceType.GetProperty(nullStringKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string;
                }
                catch
                {
                }
            }
        }
        public FormatAttribute(Type resourceType, string prefixKey, string postfixKey)
            : base()
        {
            if (resourceType == null) return;
            if (!string.IsNullOrWhiteSpace(prefixKey))
            {
                try
                {
                    Prefix = resourceType.GetProperty(prefixKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string;
                }
                catch
                {
                }
            }
            if (!string.IsNullOrWhiteSpace(postfixKey))
            {
                try
                {
                    Postfix = resourceType.GetProperty(postfixKey, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as string;
                }
                catch
                {
                }
            }
            
        }
        public FormatAttribute(DisplayFormatAttribute orig)
            : base()
        {
            this.HtmlEncode = orig.HtmlEncode;
            this.ConvertEmptyStringToNull = orig.ConvertEmptyStringToNull;
            this.DataFormatString = orig.DataFormatString;
            this.ApplyFormatInEditMode = orig.ApplyFormatInEditMode;
            this.NullDisplayText = orig.NullDisplayText;
            
        }
        public string GetDisplay(object x)
        {
            if (string.IsNullOrWhiteSpace(DataFormatString) && !(string.IsNullOrWhiteSpace(ClientFormat)))
            {
                if (Prefix == null) Prefix = string.Empty;
                if (Postfix == null) Postfix = string.Empty;
                DataFormatString = Prefix + "{0:" + (ClientFormat == "S" ? "s" : ClientFormat) + "}" + Postfix; 
            }
            if (x != null && !string.IsNullOrWhiteSpace(DataFormatString)) return string.Format(DataFormatString, x);
            return GetEdit(x);
        }
        public void GetClientFormat(out string prefix, out string postfix, out string clientFormat)
        {
            prefix = Prefix;
            if (prefix == null) prefix = string.Empty;

            postfix = Postfix;
            if(postfix == null) postfix=string.Empty;

            clientFormat = ClientFormat;
            if (clientFormat == null) clientFormat = string.Empty;
            if (!ExtractClientFormat)
            {
              /*  if (!new System.Text.RegularExpressions.Regex(@"[ndDpcsS]\d*").IsMatch(clientFormat))
                {
                    clientFormat = string.Empty;
                }*/
                return;
            }
            string[] parts=null;
            if (string.IsNullOrWhiteSpace(ClientFormat) && !string.IsNullOrWhiteSpace(DataFormatString))
            {
                parts=ClientFormat.Split(new string[] {"{0:", "}"}, StringSplitOptions.RemoveEmptyEntries );
                if (parts.Length > 1)
                {
                    clientFormat = parts[1];
                    if (clientFormat == "s") clientFormat = "S";
                }
                

            }
            /*if (!new System.Text.RegularExpressions.Regex(@"[ndpc]\d*").IsMatch(clientFormat))
            {
                clientFormat = string.Empty;
            }*/
            if (string.IsNullOrWhiteSpace(ClientFormat) && !string.IsNullOrEmpty(ClientFormat) && !string.IsNullOrWhiteSpace(DataFormatString))
            {
                prefix = parts[0].Replace("{{", "{").Replace("}}", "}");
                if (parts.Length > 2) postfix = parts[2].Replace("{{", "{").Replace("}}", "}");
            }
        }
        
        private string GetEdit(object x)
        {
            if (x == null)
            {
                if (NullDisplayText != null) return NullDisplayText;
                else return string.Empty;
            }
            else{
                string res = Convert.ToString(x);
                if (Prefix != null) res = Prefix + res;
                if (Postfix != null) res = res + Postfix;
                return res;
            }
            
        }
    }
}
