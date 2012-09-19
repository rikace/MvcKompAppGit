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

namespace MVCControlsToolkit.Core.Utilities
{
    internal static class Sorting
    {
        internal static int[] CompactArray(int[] x, int emptySymbol)
        {
            int freePlace=0;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == emptySymbol ) continue;
                if (i == freePlace)
                {
                    freePlace++;

                }
                else
                {
                    x[freePlace] = x[i];
                    freePlace++;
                }   
            }
            if(freePlace == x.Length) return x;
            int[] res = new int[freePlace];
            for (int j = 0; j < freePlace; j++) res[j] = x[j];
            return res;

        }
        internal static int[] InvertPermutationsArray(int[] perm)
        {
            int max = perm.Max();
            int[] pRes = new int[max+1];
            for (int i = 0; i < pRes.Length; i++) pRes[i] = -1;
            for (int i = 0; i < perm.Length; i++)
            {
                pRes[perm[i]] = i;
            }
            return CompactArray(pRes, -1);
        }
        internal static List<T> ApplyPermutation<T>(List<T> source, int[] permutation)
        {
            T[] pRes = new T[source.Count];
            int i = 0;
            foreach (T el in source)
            {
                pRes[permutation[i]] = el;
                i++;
            }
            return pRes.ToList<T>();
        }
        internal static int[] GetPermutationArray(string permutationString)
        {
            string[] permutationStrings = permutationString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] res = new int[permutationStrings.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = int.Parse(permutationStrings[i]);
            }
            return res;
        }
    }
}
