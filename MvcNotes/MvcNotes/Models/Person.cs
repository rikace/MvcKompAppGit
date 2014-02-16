using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcNotes.Validation;

namespace MvcNotes.Models
{
    [Bind(Exclude = "Id")]
    public class Person
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Person name is required")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(160)]
        [DisplayName("Person FirstName")]
        public string FirstName { get; set; }

        [DisplayName("Person LastName")]
        public string LastName { get; set; }

        [AgeValidation]
        [DisplayName("Age")]


        public int Age { get; set; }

        [DisplayName("Street")]
        public string Street { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("State")]
        public string State { get; set; }

        [Range(1, 100, ErrorMessage = "Zipcode must be between 10000 and 99999")]
        [DisplayName("Zipcode")]
        public int Zipcode { get; set; }

        //public Person()
        //{
        //    Id = 1;
        //    Name = "Sai";
        //    Age = 30;
        //    Street = "50 Heaven St";
        //    City = "Mansfield";
        //    State = "MA";
        //    Zipcode = 02048;
        //}
    }
}