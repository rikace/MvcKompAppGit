using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCControlsToolkit.Controls.TreeView
{
    public interface ITreeViewNodeContainer
    {
         object AddToBranch(object obranch);
         void AddAllMySons();
         void AddSon(ITreeViewNodeContainer son);
         string FatherOriginalId { get; set; }
         int PositionAsSon { get; set; }
         string OriginalId { get; set; }
         string ModelName {get; set;}
         string SonCollectionName { get; set; }
         int SonNumber { get; set; }
         bool Closed { get; set; }
         ITreeViewNodeContainer[] MySonContainers { get; set; }
    }
    
    
}
