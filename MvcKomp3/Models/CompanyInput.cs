using MvcKomp3.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcKomp3.Models
{
    public class CompanyInput
    {
        [Required]
        public string CompanyName { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        //        [DataType(DataType.Date)]
        //        [Required]
        //        public DateTime? BeginDate { get; set; }
        //
                [DataType(DataType.Date)]
                [DateComesLater("BeginDate")]
                public DateTime? EndDate { get; set; }
    }

    public class CompanyInputClient
    {
        [Required]
        public string CompanyName { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        //        [DataType(DataType.Date)]
        //        [Required]
        //        public DateTime? BeginDate { get; set; }
        //
                [DataType(DataType.Date)]
                [DateComesLaterClient("BeginDate")]
                public DateTime? EndDate { get; set; }
    }

}