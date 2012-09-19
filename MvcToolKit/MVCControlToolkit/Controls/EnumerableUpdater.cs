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
using System.Reflection;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class EnumerableUpdater<TItem> : IUpdateModel, IUpdateModelType
    {
        public TItem Item { get; set; }
        public bool Deleted{get; set;}
        protected Type FatherType, ListType;
        public EnumerableUpdater()
        {
        }
        public EnumerableUpdater(bool deleted)
        {
            this.Deleted = deleted;
        }
        public virtual object UpdateModel(object model, string[] fields)
        {
            if (Deleted) return model;
            if (model == null)
            {

                Type allListType = typeof(List<string>).GetGenericTypeDefinition().MakeGenericType(ListType);
                if (!FatherType.IsAssignableFrom(allListType))
                {
                    throw (new NotSupportedException(ControlsResources.OnlyListForUpdatableEnumerable));
                }
                model = allListType.GetConstructor(new Type[0]).Invoke(new object[0]);
                
            }
           
            (model as IList).Add(Item);
            return model;
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNames, object[] args=null)
        {

            if (model != null)
            {
                try
                {
                    Item = (TItem)model;
                }
                catch { }
            }
        }


        public void SendFatherType(Type t)
        {
            FatherType=t;
            if (FatherType.GetGenericArguments().Length == 0)
            {
                ListType = typeof(object);
            }
            else
            {
                ListType = FatherType.GetGenericArguments()[0];
            }
            if (!ListType.IsAssignableFrom(typeof(TItem)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(TItem).FullName,
                    ListType.FullName));
        }
    }

    public class SimpleEnumerableUpdater<TItem> : IUpdateModel
    {
        public TItem Item { get; set; }
        public virtual object UpdateModel(object model, string[] fields)
        {
            if (model != null && model.GetType() != typeof(List<TItem>))
                throw (new NotSupportedException(ControlsResources.OnlyListForUpdatableEnumerable));
            if (model == null) model = new List<TItem>();

            (model as List<TItem>).Add(Item);
            return model;
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNames, object[] args=null)
        {

            if (model != null)
            {
                try
                {
                    Item = (TItem)model;
                }
                catch { }
            }
            
        }

    }
    public class AutoEnumerableUpdater<TItem> : EnumerableUpdater<TItem>
    {
        public override object UpdateModel(object model, string[] fields)
        {
         /*   if(Item == null) Deleted=true;
            else 
            {
                string str= Item as string; 
                if(str != null && string.IsNullOrWhiteSpace(str)) Deleted=true;
                else
                {
                  IComparable comp=Item as IComparable;
                    if(comp != null && comp.CompareTo(default(TItem))==0) Deleted=true;
                }
            }*/
            
            return base.UpdateModel(model, fields);
        }
    }
}
