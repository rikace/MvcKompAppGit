using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKomp3.Infrastructure;

namespace MvcKompApp.Models
{
    public class Movie
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]        
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Genre must be specified")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Price Required")]
        [Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }

        [StringLength(5)]
        public string Rating { get; set; }
    }

    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }

    public class MovieInitializer : DropCreateDatabaseIfModelChanges<MovieContext>
    {
        protected override void Seed(MovieContext context)
        {
            var movies = new List<Movie> {  
  
                 new Movie { Title = "When Harry Met Sally",   
                             ReleaseDate=DateTime.Parse("1989-1-11"),   
                             Genre="Romantic Comedy",  
                             Rating="G",  
                             Price=7.99M},  

                     new Movie { Title = "Ghostbusters ",   
                             ReleaseDate=DateTime.Parse("1984-3-13"),   
                             Genre="Comedy",  
                              Rating="G",  
                             Price=8.99M},   
  
                 new Movie { Title = "Ghostbusters 2",   
                             ReleaseDate=DateTime.Parse("1986-2-23"),   
                             Genre="Comedy",  
                             Rating="G",  
                             Price=9.99M},   

               new Movie { Title = "Rio Bravo",   
                             ReleaseDate=DateTime.Parse("1959-4-15"),   
                             Genre="Western",  
                             Rating="G",  
                             Price=3.99M},   
             };

            movies.ForEach(d => context.Movies.Add(d));
            context.SaveChanges();
        }
    }
}