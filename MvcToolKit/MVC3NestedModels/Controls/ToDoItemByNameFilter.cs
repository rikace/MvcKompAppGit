using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCNestedModels.Models;
using MVCControlsToolkit.Controls;
using MVCControlsToolkit.Controls.DataFilter;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace MVCNestedModels.Controls
{
    public class ToDoItemByNameFilter: 
        IFilterDescription<ToDoItem>
    {
        [Required]
        public string Name {get; set;}
        public System.Linq.Expressions.Expression<Func<ToDoItem, bool>> GetExpression()
        {
            System.Linq.Expressions.Expression<Func<ToDoItem, bool>> res=null;
            Name=Name.Trim();
            if (!string.IsNullOrEmpty(Name))
            {
                Name=Name.Trim();
                res= m => (m.Name == Name);
                
            }
            return res;
        }
    }
}