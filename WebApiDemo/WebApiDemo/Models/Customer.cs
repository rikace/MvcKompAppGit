using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApi.Controllers;

namespace WebApi.Models
{
    public class Customer : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

    }
}