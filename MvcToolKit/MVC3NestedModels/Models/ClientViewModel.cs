using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCControlsToolkit.Core;
using System.ComponentModel.DataAnnotations;
namespace MVCNestedModels.Models
{
    public class ClientViewModel:IDisplayModel
    {
        public List<string> Keywords { get; set; }
        public List<string> SelectedKeywords { get; set; }
        [StringLength(10, ErrorMessage="The maximum length of field {0} is 10")]
        [Display(Name="Item to add", Prompt="Add new keyword")]
        public string ItemToAdd { get; set; }
        public ClientViewModel()
        {
            
            SelectedKeywords = new List<string>();
            ItemToAdd = "";
        }

        public object ExportToModel(Type TargetType, params object[] context)
        {
            return Keywords;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Keywords = model as List<string>;
            if (Keywords == null)
                Keywords = new List<string>();
        }
    }
}