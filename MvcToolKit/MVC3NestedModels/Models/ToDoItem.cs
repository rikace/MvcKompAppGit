using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;

namespace MVCNestedModels.Models
{
    public class ToDoItem
    {
       public int Code { get; set; } 

       [Required, CanSort, Display(ShortName = "NAME")]
       public string Name {get;set;}
       
        [CanSort, Display(ShortName = "DESCRIPTION")]
       public string Description { get; set; }

        [Display(ShortName = "Important")]
        public bool Important { get; set; }
        [Display(Name = "Roles in Blog(list)", Prompt = "Choose a Role")]
        public List<int> ToDoRoles { get; set; }
        [Display(Name = "Roles in Blog(single)", Prompt = "Choose a Role"), Format(NullDisplayText="No role selected")]
        public int ToDoRole { get; set; }

        
    }
    public class ToDoItemExt : ToDoItem
    {
        string SupplementaryInfo { get; set; }
    }
}