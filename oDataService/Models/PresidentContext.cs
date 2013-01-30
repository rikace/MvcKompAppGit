using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace oDataService.Models
{
    public class PresidentContext : DbContext
    {
        //static PresidentContext()
        //{
           
        //}

        public DbSet<President> Presidents { get; set; }
    }

    public class President
    {
        public int PresidentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime TookOffice { get; set; }
        public DateTime? LeftOffice { get; set; }
        public string Party { get; set; }
        public int NumTerms { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1} {2}", PresidentID, FirstName, LastName);
        }
    }

    public class DataBasePresidentsInitializer : DropCreateDatabaseIfModelChanges<PresidentContext>
    {
        protected override void Seed(PresidentContext context)
        {
            List<President> list = new List<President>
            {
                new President { PresidentID = 1, FirstName = "George", LastName = "Washington", TookOffice = new DateTime(1789, 4, 30), LeftOffice = new DateTime(1797, 3, 4), Party = "No party", NumTerms = 2},
                new President { PresidentID = 2, FirstName = "John", LastName = "Adams", TookOffice = new DateTime(1797, 3, 4), LeftOffice = new DateTime(1801, 3, 4), Party = "Federalist", NumTerms = 3},
                new President { PresidentID = 3, FirstName = "Thomas", LastName = "Jefferson", TookOffice = new DateTime(1801, 3, 4), LeftOffice = new DateTime(1809, 3, 4), Party = "Democratic-Republican", NumTerms = 2},
                new President { PresidentID = 4, FirstName = "James", LastName = "Madison", TookOffice = new DateTime(1809, 3, 4), LeftOffice = new DateTime(1817, 3, 4), Party = "Democratic-Republican", NumTerms = 2},
                new President { PresidentID = 5, FirstName = "James", LastName = "Monroe", TookOffice = new DateTime(1817, 3, 4), LeftOffice = new DateTime(1825, 3, 4), Party = "Democratic-Republican", NumTerms = 2},
                new President { PresidentID = 6, FirstName = "John", LastName = "Quincy Adams", TookOffice = new DateTime(1825, 3, 4), LeftOffice = new DateTime(1829, 3, 4), Party = "Democratic-Republican", NumTerms = 1},
                new President { PresidentID = 7, FirstName = "Andrew", LastName = "Jackson", TookOffice = new DateTime(1829, 3, 4), LeftOffice = new DateTime(1837, 3, 4), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 8, FirstName = "Martin", LastName = "Van Buren", TookOffice = new DateTime(1837, 3, 4), LeftOffice = new DateTime(1841, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 9, FirstName = "William", LastName = "Harrison", TookOffice = new DateTime(1841, 3, 4), LeftOffice = new DateTime(1841, 4, 4), Party = "Whig", NumTerms = 0},
                new President { PresidentID = 10, FirstName = "John", LastName = "Tyler", TookOffice = new DateTime(1841, 4, 4), LeftOffice = new DateTime(1845, 3, 4), Party = "Whig", NumTerms = 1},
                new President { PresidentID = 11, FirstName = "James", LastName = "Polk", TookOffice = new DateTime(1845, 4, 4), LeftOffice = new DateTime(1849, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 12, FirstName = "Zachary", LastName = "Taylor", TookOffice = new DateTime(1849, 4, 4), LeftOffice = new DateTime(1850, 7, 9), Party = "Whig", NumTerms = 0},
                new President { PresidentID = 13, FirstName = "Millard", LastName = "Fillmore", TookOffice = new DateTime(1850, 7, 9), LeftOffice = new DateTime(1853, 3, 4), Party = "Whig", NumTerms = 1},
                new President { PresidentID = 14, FirstName = "Franklin", LastName = "Pierce", TookOffice = new DateTime(1853, 3, 4), LeftOffice = new DateTime(1857, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 15, FirstName = "James", LastName = "Buchanan", TookOffice = new DateTime(1857, 3, 4), LeftOffice = new DateTime(1861, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 16, FirstName = "Abraham", LastName = "Lincoln", TookOffice = new DateTime(1861, 3, 4), LeftOffice = new DateTime(1865, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 17, FirstName = "Andrew", LastName = "Johnson", TookOffice = new DateTime(1865, 4, 15), LeftOffice = new DateTime(1869, 3, 4), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 18, FirstName = "Ulysses", LastName = "Grant", TookOffice = new DateTime(1869, 3, 4), LeftOffice = new DateTime(1877, 3, 4), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 19, FirstName = "Rutherford", LastName = "Hayes", TookOffice = new DateTime(1877, 3, 4), LeftOffice = new DateTime(1881, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 20, FirstName = "James", LastName = "Garfield", TookOffice = new DateTime(1881, 3, 4), LeftOffice = new DateTime(1881, 9, 19), Party = "Republican", NumTerms = 0},
                new President { PresidentID = 21, FirstName = "Chester", LastName = "Arthur", TookOffice = new DateTime(1881, 9, 19), LeftOffice = new DateTime(1885, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 22, FirstName = "Grover", LastName = "Cleveland", TookOffice = new DateTime(1885, 3, 4), LeftOffice = new DateTime(1889, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 23, FirstName = "Benjamin", LastName = "Harrison", TookOffice = new DateTime(1889, 3, 4), LeftOffice = new DateTime(1893, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 24, FirstName = "Grover", LastName = "Cleveland", TookOffice = new DateTime(1893, 3, 4), LeftOffice = new DateTime(1897, 3, 4), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 25, FirstName = "William", LastName = "McKinley", TookOffice = new DateTime(1897, 3, 4), LeftOffice = new DateTime(1901, 9, 14), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 26, FirstName = "Theodore", LastName = "Roosevelt", TookOffice = new DateTime(1901, 9, 14), LeftOffice = new DateTime(1909, 3, 4), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 27, FirstName = "William", LastName = "Taft", TookOffice = new DateTime(1909, 3, 4), LeftOffice = new DateTime(1913, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 28, FirstName = "Woodrow", LastName = "Wilson", TookOffice = new DateTime(1913, 3, 4), LeftOffice = new DateTime(1921, 3, 4), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 29, FirstName = "Warren", LastName = "Harding", TookOffice = new DateTime(1921, 3, 4), LeftOffice = new DateTime(1923, 8, 2), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 30, FirstName = "Calvin", LastName = "Coolidge", TookOffice = new DateTime(1920, 8, 2), LeftOffice = new DateTime(1929, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 31, FirstName = "Herbert", LastName = "Hoover", TookOffice = new DateTime(1929, 3, 4), LeftOffice = new DateTime(1933, 3, 4), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 32, FirstName = "Franklin", LastName = "Roosevelt", TookOffice = new DateTime(1933, 3, 4), LeftOffice = new DateTime(1945, 4, 12), Party = "Democratic", NumTerms = 4},
                new President { PresidentID = 33, FirstName = "Harry", LastName = "Truman", TookOffice = new DateTime(1945, 4, 12), LeftOffice = new DateTime(1953, 1, 20), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 34, FirstName = "Dwight", LastName = "Eisenhower", TookOffice = new DateTime(1953, 1, 20), LeftOffice = new DateTime(1961, 1, 20), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 35, FirstName = "John", LastName = "Kennedy", TookOffice = new DateTime(1961, 1, 20), LeftOffice = new DateTime(1963, 11, 22), Party = "Democratic", NumTerms = 0},
                new President { PresidentID = 36, FirstName = "Lyndon", LastName = "Johnson", TookOffice = new DateTime(1963, 11, 22), LeftOffice = new DateTime(1969, 1, 20), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 37, FirstName = "Richard", LastName = "Nixon", TookOffice = new DateTime(1969, 1, 20), LeftOffice = new DateTime(1974, 8, 9), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 38, FirstName = "Gerald", LastName = "Ford", TookOffice = new DateTime(1974, 8, 9), LeftOffice = new DateTime(1977, 1, 20), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 39, FirstName = "Jimmy", LastName = "Carter", TookOffice = new DateTime(1977, 1, 20), LeftOffice = new DateTime(1981, 1, 20), Party = "Democratic", NumTerms = 1},
                new President { PresidentID = 40, FirstName = "Ronald", LastName = "Reagan", TookOffice = new DateTime(1981, 1, 20), LeftOffice = new DateTime(1989, 1, 20), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 41, FirstName = "George", LastName = "Bush", TookOffice = new DateTime(1989, 1, 20), LeftOffice = new DateTime(1993, 1, 20), Party = "Republican", NumTerms = 1},
                new President { PresidentID = 42, FirstName = "Bill", LastName = "Clinton", TookOffice = new DateTime(1993, 1, 20), LeftOffice = new DateTime(2001, 1, 20), Party = "Democratic", NumTerms = 2},
                new President { PresidentID = 43, FirstName = "George", LastName = "Bush", TookOffice = new DateTime(2001, 1, 20), LeftOffice = new DateTime(2009, 1, 20), Party = "Republican", NumTerms = 2},
                new President { PresidentID = 44, FirstName = "Barack", LastName = "Obama", TookOffice = new DateTime(2009, 1, 20), Party = "Democratic", NumTerms = 1}
            };

            foreach (var p in list)
            {
                context.Presidents.Add(p);
            }
            context.SaveChanges();
            base.Seed(context);
        }
    }
}