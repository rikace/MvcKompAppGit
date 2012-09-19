using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;
using MVCControlsToolkit.Core;
namespace MVCNestedModels.Models
{
    public class EmailElement: ISafeCreation
    {
        [Required, Format(NullDisplayText="empty name")]
        public string Name {get; set;}
        
    }
    public class EmailDocument : EmailElement
    {
        public string Content { get; set; }
    }
    public class EmailFolder : EmailElement
    {
        public IList<EmailElement> Children { get; set; } 
    }
}