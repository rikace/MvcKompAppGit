using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MvcKompApp.Validation;

namespace MvcKompApp.Models
{
    public class PersonRemoteValidationModel
    {
        [Required()]
        [DisplayName("User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [RemoteUID_(Controller = "RemoteValidation", Action = "IsUID_Available", ParameterName = "candidate")]
        [ScaffoldColumn(false)]
        public string UserName { get; set; }
       
        [Required()]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required()]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string City { get; set; }
    }
}