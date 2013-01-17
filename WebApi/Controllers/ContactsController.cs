using KnockoutJS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Controllers;

namespace KnockoutJS.Controllers
{
    public class ContactsController : HttpApiController<Contact>
    {
        private static List<Contact> source = new[]
                {
                    new Contact { Id = 1, Name = "Roberto Hernandez" },
                    new Contact { Id = 2, Name = "Isaura Hernandez" },
                    new Contact { Id = 3, Name = "Regina Hernandez" },
                    new Contact { Id = 4, Name = "Marianela Giraldez" },
                }
                .ToList();

        public ContactsController()
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
