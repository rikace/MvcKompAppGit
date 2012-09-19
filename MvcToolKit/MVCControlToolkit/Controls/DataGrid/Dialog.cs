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

namespace MVCControlsToolkit.Controls.DataGrid
{
    public class Dialog
    {
        public bool Modal { get; set; }//default false, says if the Dialog is modal
        public bool Stack { get; set; }//default true, if true the dialog will stack on top of other dialog
        public string Title { get; set; }//default string.empty, the title of the Dialog
        public string DialogClass { get; set; }//default null, a css class to add to the dialog
        public bool Draggable { get; set; }//default true, specifies if the dialog is draggable
        public bool Resizable { get; set; }//default true, if resizable
        public string Show { get; set; }//default null, specifies the name of the effect to apply when dialog appear
        public string Hide { get; set; }//default null, specifies the name of the effect to apply when dialog disappear
        public bool CloseOnEscape { get; set; }//default true, the dialog is closed when escape is hit
        public int? MaxHeight { get; set; }//default null
        public int? MinHeight { get; set; }//default null
        public int? MaxWidth { get; set; }//default null
        public int? MinWidth { get; set; }//default null
        public string Position { get; set; }//default 'center', position on the screen. Not numeric positions need to be inclused between ''
        public int? InitialZIndex { get; set; }//default null, initial ZIndex
        

        public Dialog()
        {
            Stack = true;
            Title = string.Empty;
            Draggable = true;
            CloseOnEscape = true;
            Resizable = true;
            Position = "'center'";
        }
        public string GetCreation(string selector)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function()
                {");
            sb.Append("$( '");
            sb.Append(selector);
            sb.Append("').dialog({ autoOpen: false");
            //init options
            if (Modal) sb.Append(", modal: true");
            if (Show != null)
            {
                sb.Append(", show: '");
                sb.Append(Show);
                sb.Append("'");
            }
            if (Hide != null)
            {
                sb.Append(", hide: '");
                sb.Append(Hide);
                sb.Append("'");
            }
            if (!Stack) sb.Append(", stack: false");
            if (!Draggable) sb.Append(", draggable: false");
            if (!Resizable) sb.Append(", resizable: false");
            if (!CloseOnEscape) sb.Append(", closeOnEscape: false");
            
            sb.Append(", title: '");  
            sb.Append(Title);
            sb.Append("'");

            if (DialogClass != null)
            {
                sb.Append(", dialogClass: '");
                sb.Append(DialogClass);
                sb.Append("'");
            }

            if (Position != null)
            {
                sb.Append(", position: ");
                sb.Append(Position);
            }
            if (InitialZIndex.HasValue)
            {
                sb.Append(", zIndex: ");
                sb.Append(InitialZIndex.Value);
            }
            if (MinHeight.HasValue)
            {
                sb.Append(", minHeight: ");
                sb.Append(MinHeight.Value);
            }
            if (MaxHeight.HasValue)
            {
                sb.Append(", maxHeight: ");
                sb.Append(MaxHeight.Value);
            }
            if (MinWidth.HasValue)
            {
                sb.Append(", minWidth: ");
                sb.Append(MinWidth.Value);
            }
            if (MaxWidth.HasValue)
            {
                sb.Append(", maxWidth: ");
                sb.Append(MaxWidth.Value);
            }
            //end options
            sb.Append("});});");//close instruction and ready function
            
            sb.Append("</script>");
            return sb.ToString();
        }
        public string GetShow(string selector)
        {
            return string.Format(" $('{0}').dialog(\"open\"); ", selector);
        }
    }
}
