using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcKompApp.ViewModels
{
    public class PersonViewModel
    {
        [DisplayName("Company ID")]
        [ReadOnly(true)]
        public Int32 Id { get; set; }

        [DisplayName("Company or individual")]
        public Boolean IsCompany { get; set; }

        [DisplayFormat(NullDisplayText = "(empty)")]
        [Required]
        public String Name { get; set; }

        [DataType(DataType.MultilineText)]
        public String Notes { get; set; }

        [UIHint("my_date_template")]
        public DateTime Foundation { get; set; }

        [DataType(DataType.Url)]
        public String Website { get; set; }

        [DisplayName("Reliable?")]
        public Boolean? Reliable { get; set; }

        public ContactInfo Contact { get; set; }
    }

    public class ContactInfo
    {
        public String FullName { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }
        public override string ToString()
        {
            return "This is what I am";
        }
    }
}