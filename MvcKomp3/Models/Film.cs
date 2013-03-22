using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKomp3.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public DateTime ReleasedOn { get; set; }
        public string ImageUrl { get; set; }
    }
}