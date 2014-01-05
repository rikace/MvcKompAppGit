using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceWebApi.Controllers
{
    public class VideoDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Uri Link { get; set; }
    }
}
