using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.DAL
{
    public interface IUserDB
    {
        List<User> GetAllUsers { get; }
        bool UserExists(string UserName);
        void CreateNewUser(User newUser);
        User GetUser(string uid);
        void Remove(string usrName);
        void Update(User userToUpdate);
    }

    public class User:Customer
    {
        public string UserName { get; set; } 
    }
}