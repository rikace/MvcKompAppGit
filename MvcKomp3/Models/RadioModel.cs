using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompApp.Models
{
    public class RadioModel:IValidatableObject
    {
        public IEnumerable<SelectListItem> TestRadioList { get; set; }

        [Required(ErrorMessage = "You must select an option for TestRadio")]
        public String TestRadio { get; set; }

        public List<String> Items { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TestRadio == "3")
                yield return new ValidationResult("Error in TestRadio");
        }
    }

    public class aTest
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
    }
}