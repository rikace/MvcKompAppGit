using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Validation;

namespace MvcKompApp.Models
{
    public class Appointment
    {

        //[AllowHtml]
        //[EmailAddress]
        [Remote("ValidateClientName", "Appointment")]
        public string ClientName { get; set; }

        [DataType(DataType.Date)]
        [Remote("ValidateDate", "Appointment")]
        public DateTime Date { get; set; }


        [MustBeTrue]
        public bool TermsAccepted { get; set; }
    }
}