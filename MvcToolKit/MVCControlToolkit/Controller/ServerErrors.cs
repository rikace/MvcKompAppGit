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
using MVCControlsToolkit.Core;


namespace MVCControlsToolkit.Controller
{
    public class ServerError
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<string> errors { get; set; }
    }
    public class KeyInfos<T>
    {
        public string destinationExpression { get; set; }
        public T[] keys { get; set; }
    }
    public class ServerErrors
    {
        public List<ServerError> errors { get; set; }
        private List<string> errorList(ModelState entry)
        {
            List<string> res = new List<string>();
            foreach (var x in entry.Errors)
            {
                if (x.ErrorMessage != null) res.Add(x.ErrorMessage);
                else if (x.Exception != null) res.Add(x.Exception.Message);
            }
            if (res.Count == 0) res.Add("");
            return res;
        }
        public ServerErrors(ModelStateDictionary origin, string prefix = null)
        {
            List<ServerError> res = new List<ServerError>();
            
            foreach (KeyValuePair<string, ModelState> pair in origin)
            {
                if (pair.Value.Errors.Count > 0 && (prefix == null || pair.Key.StartsWith(prefix)))
                {
                    res.Add(new ServerError
                    {
                        name = pair.Key,
                        id = BasicHtmlHelper.IdFromName(pair.Key),
                        errors = errorList(pair.Value)
                    });
                }
            }
            
            errors = res;
        }
        
    }
    public class ServerErrors<T> : ServerErrors
    {
        public KeyInfos<T>[] insertedKeys{get; set;}
        public ServerErrors(ModelStateDictionary origin, KeyInfos<T>[] insertedKeys, string prefix = null):base(origin, prefix)
        {
            this.insertedKeys=insertedKeys;
        }
        public ServerErrors(ModelStateDictionary origin, T[] keys, string destinationExpression=null, string prefix = null)
            : base(origin, prefix)
        {
            this.insertedKeys = new KeyInfos<T>[] { new KeyInfos<T> { keys = keys, destinationExpression = destinationExpression } };
        }
    }
    public static class ServerErrorHelper
    {
        public static ServerErrors GetErrors(ModelStateDictionary modelState)
        {
            if (modelState == null) throw new ArgumentNullException("modelState");
            return new ServerErrors(modelState, null);
        }

        public static ServerErrors GetErrors<M, T>(M viewModel, Expression<Func<M, T>> expression, ModelStateDictionary modelState)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (modelState == null) throw new ArgumentNullException("modelState");
            if (expression == null) throw new ArgumentNullException("expression");
            
            return new ServerErrors(modelState, ExpressionHelper.GetExpressionText(expression));
        }
    }
}
