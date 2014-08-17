using MvcKompApp.Models;
using MvcKompMvc4.ServiceControllers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompMvc4.ServiceControllers.Home
{
    public class HomeService:IHomeService
    {
        public List<FavouriteGivenName> GetFavouriteGiveNames()
        {
            var mostPopular = new List<FavouriteGivenName>()
            {
                 new FavouriteGivenName() {Id = 1, Name = "Jack", Age = 30},
                new FavouriteGivenName() {Id = 2, Name = "Riley", Age = 40},
                new FavouriteGivenName() {Id = 3, Name = "William", Age = 17},
                new FavouriteGivenName() {Id = 4, Name = "Oliver", Age = 56},
                new FavouriteGivenName() {Id = 5, Name = "Lachlan", Age = 25},
                new FavouriteGivenName() {Id = 6, Name = "Thomas", Age = 75},
                new FavouriteGivenName() {Id = 7, Name = "Joshua", Age = 93},
                new FavouriteGivenName() {Id = 8, Name = "James", Age = 15},
                new FavouriteGivenName() {Id = 9, Name = "Liam", Age = 73},
                new FavouriteGivenName() {Id = 10, Name = "Max", Age = 63}
            };
            return mostPopular;
        }
    }
}