using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.DAL
{
    public class InMemoryDB : IUserDB
    {
        private static InMemoryDB _instance;

        public InMemoryDB()
        {
            GetAllUsers = new List<User> {
                new User{ UserName = "BenM", FirstName = "Ben", LastName = "Miller",   Age=38, Id=1},
                new User{ UserName = "AnnB", FirstName = "Ann", LastName = "Beebe",  Age=9, Id=2}
            };
        }

        public static InMemoryDB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InMemoryDB();
                return _instance;
            }
        }

        public List<User> GetAllUsers
        {
            get;
            private set;
        }

        public bool UserExists(string UserName)
        {
            foreach (User usr in GetAllUsers)
            {
                if (string.Equals(usr.UserName, UserName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }


        public User GetUser(string uid)
        {

            foreach (User u in GetAllUsers)
                if (u.UserName == uid)
                    return u;

            return null;
        }

        public void Update(User userToUpdate)
        {

            foreach (User usr in GetAllUsers)
            {
                if (usr.UserName == userToUpdate.UserName)
                {
                    GetAllUsers.Remove(usr);
                    GetAllUsers.Add(userToUpdate);
                    break;
                }
            }
        }

        public void CreateNewUser(User newUser)
        {
            foreach (User usr in GetAllUsers)
            {
                if (usr.UserName.ToLowerInvariant() == newUser.UserName.Trim().ToLowerInvariant())
                {
                    throw new System.InvalidOperationException("Duplicate username: " + usr.UserName);
                }
            }
            GetAllUsers.Add(newUser);
        }

        public void Remove(string usrName)
        {

            foreach (User usr in GetAllUsers)
            {
                if (usr.UserName == usrName)
                {
                    GetAllUsers.Remove(usr);
                    break;
                }
            }
        }

    }

}