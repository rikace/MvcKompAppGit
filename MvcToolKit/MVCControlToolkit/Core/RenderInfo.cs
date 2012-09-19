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

namespace MVCControlsToolkit.Core
{
    public class RenderInfo
    {
        string _PartialRendering = null;
        internal string PartialRendering
        {
            get
            {
                if (_PartialRendering == null) return null;
                string temp = _PartialRendering;
                _PartialRendering = string.Empty;
                return temp;
            }
            set
            {
                _PartialRendering = value;
            }
        }
        public string Prefix { get; set; }
        public string PartialPrefix { get; set; }
        public string GetPartialrendering() { return PartialRendering; }
    }
    public class RenderInfo<M>: RenderInfo
    {
        public RenderInfo()
        {
        }
        public RenderInfo(string prefix, string partialPrefix, string partialRendering, M model)
        {
            Prefix = prefix;
            PartialPrefix = partialPrefix;
            PartialRendering = partialRendering;
            Model = model;
        }
       
        public M Model {get; set;}    
    }
}
