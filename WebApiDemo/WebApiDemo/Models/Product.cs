using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Controllers;

namespace HelloWebAPI.Models
{
    public class Product:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}