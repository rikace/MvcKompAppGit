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
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class NullableUpdater<T> : IUpdateModel, IUpdateModelType
        where T: struct
    {
        public T Item{get;set;}
        public bool Chosen { get; set;}

        public object UpdateModel(object model, string[] fields)
        {
            if (!Chosen) return model;
            else
            {
                Nullable<T> res = Item;
                return res;
            }
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNams, object[] args = null)
        {
            if (model == null || !((Nullable<T>)model).HasValue)
            {
                Item = default(T);
                Chosen = false;
            }
            else
            {
                Item = ((Nullable<T>)model).Value;
                Chosen = true;
            }
        }
        public void SendFatherType(Type t)
        {

            if (!t.IsAssignableFrom(typeof(T)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(T).FullName,
                    t.FullName));
        }
    }
    public class ReferenceTypeUpdater<T> : IUpdateModel, IUpdateModelType
        where T : class
    {
        public T Item { get; set; }
        public bool Chosen { get; set; }

        public object UpdateModel(object model, string[] fields)
        {
            if (!Chosen) return model;
            else
            {
                
                return Item;
            }
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNams, object[] args = null)
        {
            Item = model as T;
            Chosen = model != null;

        }
        public void SendFatherType(Type t)
        {

            if (!t.IsAssignableFrom(typeof(T)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(T).FullName,
                    t.FullName));
        }
    }

    internal class SubClassCastTransformer<T> : IDisplayModel, IUpdateModelType
         where T : class
    {
        public T Item { get; set; }
        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (TargetType.IsAssignableFrom(typeof(T))) return Item;
            else return null;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Item = model as T;
        }

        public void SendFatherType(Type t)
        {
            
            if (!t.IsAssignableFrom(typeof(T)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(T).FullName,
                    t.FullName));
        }
    }
   
}
