using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Controllers;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class ContactsHelperController : HttpApiController<Contact>
    {
        private static List<Contact> source = new[]
                {
                    new Contact { Id = 1, FirstName = "Roberto", LastName="Hernandez" },
                    new Contact { Id = 2, FirstName= "Isaura", LastName=" Hernandez" },
                    new Contact { Id = 3, FirstName = "Regina", LastName=" Hernandez" },
                    new Contact { Id = 4, FirstName = "Marianela", LastName=" Giraldez" },
                }
                .ToList();

        public ContactsHelperController()
            : base(source)
        {
        }

        protected override void OnHtttpPostHandled(Contact entity)
        {
            base.OnHtttpPostHandled(entity);

            entity.Id = this.Source.Any() ? this.Source.Max(e => e.Id) + 1 : 1;
        }
    }
}
   