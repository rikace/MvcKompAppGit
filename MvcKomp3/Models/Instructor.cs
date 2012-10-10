using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    [MetadataType(typeof(InstructorMetadata))]
    public partial class Instructor : Person
    { }

    public partial class Instructor
    {
        public DateTime? HireDate { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }

    public class InstructorMetadata
    {
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Hire date is required.")]
        [Display(Name = "Hire Date")]
        public object HireDate { get; set; }
    }
}