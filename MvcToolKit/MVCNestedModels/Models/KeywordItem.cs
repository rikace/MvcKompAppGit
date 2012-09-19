using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCNestedModels.Models
{
    public class KeywordItem
    {
        [Required, 
        MVCControlsToolkit.DataAnnotations.CanSort, 
        MVCControlsToolkit.DataAnnotations.Format(NullDisplayText="empty keyword")]
        public string Keyword { get; set; }
        [Required, 
        MVCControlsToolkit.DataAnnotations.CanSort,
         MVCControlsToolkit.DataAnnotations.Format(NullDisplayText = "empty title")]
        public string Title { get; set; }
    }
    public class KeywordItemExt : KeywordItem
    {
        public string SupplementaryInfos { get; set; }
    }
}