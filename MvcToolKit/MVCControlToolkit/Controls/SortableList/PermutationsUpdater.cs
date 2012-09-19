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
using System.Reflection;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Core.Utilities;
namespace MVCControlsToolkit.Controls
{
    internal class PermutationsUpdater<TItem>: IUpdateModel, IUpdateModelState
    {

        public string Permutation { get; set; }
        public object UpdateModel(object model, string[] fields)
        {
            if (model == null) return null;
            if (model.GetType() != typeof(List<TItem>))
                throw (new NotSupportedException(ControlsResources.OnlyListForUpdatableEnumerable));
            if (string.IsNullOrWhiteSpace(Permutation)) return model;
            int[] permutationArray = Sorting.InvertPermutationsArray(
                Sorting.GetPermutationArray(Permutation));
            updateModelState(permutationArray);
            return Sorting.ApplyPermutation(model as List<TItem>, permutationArray);
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNams, object[] args = null)
        {
            Permutation = string.Empty;
        }

        private string currPrefix;
        private string startPrefix;
        private int updateIndex;
        private System.Web.Mvc.ModelStateDictionary modelState;

        public void GetCurrState(string currPrefix, int updateIndex, System.Web.Mvc.ModelStateDictionary modelState)
        {
            this.currPrefix = currPrefix;
            this.updateIndex = updateIndex;
            this.modelState=modelState;
            startPrefix = ".$$";
            if (!string.IsNullOrWhiteSpace(currPrefix) && currPrefix != "updatemodel")
            {
                startPrefix = currPrefix + startPrefix;
            }
            if (startPrefix[0] == '.') startPrefix = startPrefix.Substring(1);
        }

        private void updateModelState(int[] permutationArray)
        {
            if (updateIndex < 2) return;
            List<KeyValuePair<string, System.Web.Mvc.ModelState>> register = new List<KeyValuePair<string, System.Web.Mvc.ModelState>>();
            foreach (KeyValuePair<string, System.Web.Mvc.ModelState> pair in modelState)
            {
                if(pair.Key.StartsWith(startPrefix))
                    register.Add(new KeyValuePair<string, System.Web.Mvc.ModelState>(pair.Key, pair.Value));
            }
            foreach (KeyValuePair<string, System.Web.Mvc.ModelState> pair in register)
            {
                string postFix;
                int index = indexOf(pair.Key, out postFix);
                if (index < updateIndex)
                {
                    modelState[itemPrefix(currPrefix, permutationArray[index]) + postFix] = pair.Value;
                }
                
            }
        }
        public bool MoveState { get { return true; } }
        private string itemPrefix(string prefix, int index)
        {
            string res = string.Format(".$${0}", index);
            if (!string.IsNullOrWhiteSpace(prefix) && prefix != "updatemodel")
            {
                res = prefix + res;
            }
            if (res[0] == '.') res = res.Substring(1);
            return res;
        }

        private int indexOf(string prefix, out string postFix)
        {
            string res=null;
            postFix = string.Empty;
            if (prefix.StartsWith(startPrefix))
            {
                res = prefix.Substring(startPrefix.Length);
            }
            else return -1;
            int stop = res.IndexOf('.');
            if (stop > 0)
            {
                postFix = res.Substring(stop);
                res = res.Substring(0, stop);
            }
            return int.Parse(res);
            
        }
    }
}
