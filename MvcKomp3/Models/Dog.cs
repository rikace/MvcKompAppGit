using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcKompApp.Models
{
    public class Dog
    {
        [Required(ErrorMessage = "ID is required")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required"),
        StringLength(15, ErrorMessage = "Name cannot be more than 15 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required"),
        StringLength(6, MinimumLength = 1)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Age is required"),
        Range(0, 20, ErrorMessage = "Must be between 0 and 20")]
        public int Age { get; set; }

        public bool SpayedNeutered { get; set; }

        [Required(ErrorMessage = "Please select a handedness option")]
        public string Handedness { get; set; }

        public string Notes { get; set; }
    }
}