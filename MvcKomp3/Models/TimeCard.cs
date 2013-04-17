using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAnnotations.Components;

namespace MvcKomp3.Models
{
    public class TimeCard : IValidatableObject
    {
        public TimeCard()
        {
            StartDate = new DateTime(2010, 1, 1);
            EndDate = new DateTime(2010, 1, 1);
        }

        public Guid Id { get; set; }
        [Required()]
        [StringLength(25)]
        [Remote("CheckUsername", "Home", ErrorMessage = "Username is invalid")]
        public string Username { get; set; }

        [Range(1, 120)]
        public int Hours { get; set; }

        [Range(1, 120)]
       // [Compare("Hours")]
        public int ConfirmHours { get; set; }

        public DateTime StartDate { get; set; }

        [GreaterThanDate("StartDate")]
        public DateTime EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult("EndDate must be greater than StartDate");
            }
        }
    }
}