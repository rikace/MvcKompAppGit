using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCNestedModels.Models
{
    public class RoleInBlog
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int? GroupCode { get; set; }
        public string GroupName { get; set; }
    }
}