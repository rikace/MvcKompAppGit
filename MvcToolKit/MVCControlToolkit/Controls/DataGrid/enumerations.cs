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

namespace MVCControlsToolkit.Controls
{
    public enum ItemContainerType {div, span, tr, td, p, li, section, article, dynamic}
    public enum ExternalContainerType { div, span, table, tr, td, th, p, ul, ol, li, section, article, tbody, thead, tfoot, menu, nav, koComment}
    public enum DataButtonType { Edit, Cancel, Delete, Insert, Undelete, ResetRow}
    public enum SortButtonStyle { Button, Link }
    public enum DetailType { Display, Edit }
    
    
    
    [Flags]
    public enum FieldFeatures {None = 0, Sort=1, Filtering=2, Hidden=4}
    
    [Flags]
    public enum GridFeatures {None = 0, Edit=1, Display=2, Insert=4, Delete=8, ResetRow=16, UndoEdit=32, InsertOne=64, Paging=128, Sorting=256, Filtering=512, SortingOne=1024, SortingCausesPost=2048}
    [Flags]
    public enum SortableListFeatures {None = 0, Insert=1, Delete=2, InsertOne=4, Paging=8, Sorting=16, MouseDraggingSort=64, SortingOne=128, SortingCausesPost=256}
}
