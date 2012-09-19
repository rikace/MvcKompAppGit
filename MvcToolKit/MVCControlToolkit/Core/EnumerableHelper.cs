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
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Globalization;

namespace MVCControlsToolkit.Core
{
    static public class EnumerableHelper
    {
        public static string CreateSubIndexName(string prefix, int index)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
        }
        static public IEnumerable Create<TItem>(Type t, int size)
            
        {
            
            if (t.IsInterface)
            {
                if (typeof(List<TItem>).GetInterfaces().Contains<Type>(t)) return new List<TItem>(size) as IEnumerable;
                else if (t == typeof(ISet<TItem>)) return new HashSet<TItem>();
                else throw new NotSupportedException(string.Format(Resources.NotSupportedEnumerable, t.Name));
            }
            else
            {
                if (typeof(List<TItem>) == t) return new List<TItem>(size) as IEnumerable;
                else if (t.IsArray) return new TItem[size];
                else
                {
                    ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(int) });
                    if (ci != null)
                    {
                        return ci.Invoke(new object[] { size }) as IEnumerable;
                    }
                    else
                    {
                        ci = t.GetConstructor(new Type[0]);
                        if (ci != null)
                        {
                            return ci.Invoke(new object[0]) as IEnumerable;
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format(Resources.NoConstructor, t.Name));
                        }
                    }
                }
                
            }
            
        }

        static public IEnumerable CreateFrom<TItem>(Type targetType, IEnumerable source, int size)     
        {
            if (source == null) return null;
            if (source.GetType() == targetType) return source;
           
            else
            {
                IEnumerable res = Create<TItem>(targetType, size);
                if (res is TItem[])
                {
                    int index=0;
                    TItem[] v = res as TItem[];
                    foreach (object o in source)
                    {
                        v[index] = (TItem)o;
                        index++;
                    }
                }
                else
                    Copy<TItem>(res, source);
                return res;
            }
        }

        public static void CopyFrom<TItem>(this IEnumerable destination, IEnumerable source)
        {
            Copy<TItem>(destination, source);
        }

        public static void Copy<TItem>(IEnumerable destination, IEnumerable source)
        {
            
            Type t = destination.GetType();

            MethodInfo mi = t.GetMethod("Add", new Type[] { typeof(TItem) });
            if (mi != null)
            {
                ParameterInfo[] pi = mi.GetParameters();
                if(pi.Length == 1)
                {
                    foreach(object o in source)
                    {
                        object res = mi.Invoke(destination, new object[]{o});
                        if (mi.ReturnType != null && mi.ReturnType == typeof(bool) && (bool)res == false)
                            throw (new Exception(Resources.EnumerableCopyError));
                        
                    }
                    return;
                }

            }
            mi = t.GetMethod("AddLast", new Type[] { typeof(TItem) });
            if (mi != null)
            {
                ParameterInfo[] pi = mi.GetParameters();
                if (pi.Length == 1)
                {
                    foreach (object o in source)
                    {
                        object res = mi.Invoke(destination, new object[] { o });
                        if (mi.ReturnType != null && mi.ReturnType == typeof(bool) && (bool)res == false)
                            throw (new Exception(Resources.EnumerableCopyError));
                    }
                    return;
                }
            }
            throw (new NotSupportedException(Resources.NoAddInEnumerable));
        }
    }
}
