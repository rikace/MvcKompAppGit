using MvcKompApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompMvc4.ServiceControllers.Shared
{
    public interface IHomeService
    {
        List<FavouriteGivenName> GetFavouriteGiveNames();
    }
}