using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.DAL.DataAccess
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}