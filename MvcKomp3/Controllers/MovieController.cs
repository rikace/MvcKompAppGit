using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKomp3.Models;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class MovieController : Controller
    {
        private MovieContext db = new MovieContext();
        private readonly List<Film> _movies;

        public MovieController()
        {
            _movies = new List<Film>
      {
        new Film
        { 
          Id = 1,
          Name = "Gladiator", 
          Actors = "Russell Crowe, Joaquin Phoenix, Connie Nielsen, Ralf Möller, Oliver Reed, Djimon Hounsou, Derek Jacobi, John Shrapnel and Richard Harris", 
          Director = "Ridley Scott", 
          ReleasedOn = DateTime.Parse("5/5/2000"),
          ImageUrl = "/Content/Movies/gladiator.jpg"
        },
        new Film
        { 
          Id = 2,
          Name = "Titanic", 
          Actors = "Leonardo DiCaprio, Kate Winslet", 
          Director = "James Cameron", 
          ReleasedOn = DateTime.Parse("11/1/1997"),
          ImageUrl = "/Content/Movies/titanic.jpg"
        },
        new Film
        { 
          Id = 3,
          Name = "Avatar", 
          Actors = "Sam Worthington, Zoe Saldana, Stephen Lang, Michelle Rodriguez, Sigourney Weaver, Joel David Moore, Giovanni Ribisi", 
          Director = "James Cameron", 
          ReleasedOn = DateTime.Parse("5/5/2009"),
          ImageUrl = "/Content/Movies/avatar.jpg"
        },
        new Film
        { 
          Id = 4,
          Name = "Prometheus", 
          Actors = "Noomi Rapace, Michael Fassbender, Guy Pearce, Idris Elba, Logan Marshall-Green, Charlize Theron", 
          Director = "Ridley Scott", 
          ReleasedOn = DateTime.Parse("6/1/2012"),
          ImageUrl = "/Content/Movies/prometheus.jpg"
        },
        new Film
        { 
          Id = 5,
          Name = "The Amazing Spider-Man", 
          Actors = "Andrew Garfield, Emma Stone, Rhys Ifans, Denis Leary, Martin Sheen, Sally Field, Irrfan Khan, Chris Zylka",
          Director = "Marc Webb", 
          ReleasedOn = DateTime.Parse("6/3/2012"),
          ImageUrl = "/Content/Movies/spiderman4.jpg"
        }
      };
        }


        // GET: /Movies/SearchIndex
#if ONE
public ActionResult SearchIndex(string Genre, string searchString)
{

    var GenreLst = new List<string>();
    GenreLst.Add("All");

    var GenreQry = from d in db.Movies
                   orderby d.Genre
                   select d.Genre;
    GenreLst.AddRange(GenreQry.Distinct());
    ViewBag.Genre = new SelectList(GenreLst);

    var movies = from m in db.Movies
                 select m;

    if (!String.IsNullOrEmpty(searchString))
    {
        movies = movies.Where(s => s.Title.Contains(searchString));
    }

    if (string.IsNullOrEmpty(Genre) || Genre == "All")
        return View(movies);
    else
    {
        return View(movies.Where(x => x.Genre == Genre));
    }

}
#else
        public ActionResult SearchIndex(string movieGenre, string searchString)
        {

            var GenreLst = new List<string>();

            var GenreQry = from d in db.Movies
                           orderby d.Genre
                           select d.Genre;
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.movieGenre = new SelectList(GenreLst);

            var movies = from m in db.Movies
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }


            if (string.IsNullOrEmpty(movieGenre))
                return View(movies);
            else
            {
                return View(movies.Where(x => x.Genre == movieGenre));
            }

        }
#endif

        #region Search Film With Ajax Text
        public ActionResult FilmIndex()
        {
            return View();
        }

        public PartialViewResult Films()
        {
            return PartialView("_Films",_movies);
        }
        public PartialViewResult Search(string search)
        {
            var movies = string.IsNullOrEmpty(search) ? _movies : _movies.Where(m => m.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)).Select(m => m).ToList();
            return PartialView("_Films", movies);
        } 
        #endregion

        //public ActionResult SearchIndex(string searchString)
        //{          
        //    var movies = from m in db.Movies
        //                 select m;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        movies = movies.Where(s => s.Title.Contains(searchString));
        //    }

        //    return View(movies);
        //}

        [HttpPost]
        public string SearchIndex(FormCollection fc, string searchString)
        {
            return "<h3> From [HttpPost]SearchIndex: " + searchString + "</h3>";
        }

        //
        // GET: /Movie/

        public ViewResult Index(string year = null)
        {
            ViewBag.Years = db.Movies.Select(m => m.ReleaseDate.Year).ToArray();

            if (string.IsNullOrWhiteSpace(year))
                return View(db.Movies.ToList());
            else
            {
                var yearNum = Convert.ToInt32(year);
                return View(db.Movies.Where(m => m.ReleaseDate.Year == yearNum).Select(m => m).ToList());
            }
        }

        //
        // GET: /Movie/Details/5

        public ViewResult Details(int id)
        {
            Movie movie = db.Movies.Find(id);
            return View(movie);
        }

        //
        // GET: /Movie/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Movie/Create

        [HttpPost]
        public ActionResult Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        //
        // GET: /Movie/Edit/5

        public ActionResult Edit(int id)
        {
            Movie movie = db.Movies.Find(id);
            return View(movie);
        }

        //
        // POST: /Movie/Edit/5

        [HttpPost]
        public ActionResult Edit(Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        //
        // GET: /Movie/Delete/5

        public ActionResult Delete(int id)
        {
            Movie movie = db.Movies.Find(id);
            return View(movie);
        }

        //
        // POST: /Movie/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}