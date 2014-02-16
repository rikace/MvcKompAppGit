using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using MvcNotes.Infrastructure;
using MvcNotes.Models;

namespace MvcNotes.Mapping
{
    public class PersonMapper
    {
        public static Person MapFromPresident(President president)
        {
            return Mapper.Map<President, Person>(president);
        }
    }
}