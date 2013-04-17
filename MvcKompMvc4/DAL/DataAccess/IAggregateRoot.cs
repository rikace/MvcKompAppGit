using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.DAL.DataAccess
{
    public interface IAggregateRoot : IEntity
    {
        DateTime CreatedAt { get; set; }

        DateTime? UpdatedAt { get; set; }
    }
}