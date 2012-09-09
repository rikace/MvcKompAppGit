using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.Framework
{
    internal static class UserNameHelper
    {
        private static List<string> _userNames = new List<string>();

        public static bool IsAvailable(string candidate)
        {

            foreach (PersonRemoteValidationModel um in UsrLstContainer.getUsrLst())
            {
                if (string.Equals(um.UserName, candidate, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }

    public static class UsrLstContainer
    {
        static UserList _usrLst2 = new UserList();
        public static List<PersonRemoteValidationModel> getUsrLst()
        {
            return _usrLst2._usrList;
        }
        public static UserList getUsrLstContainer()
        {
            return _usrLst2;
        }
    }

    public class UserList
    {

        public UserList()
        {
            _usrList.Add(new PersonRemoteValidationModel
            {
                UserName = "Ben",
                FirstName = "Ben",
                LastName = "Miller",
                //           Age = 33,
                City = "Seattle"
            });
            _usrList.Add(new PersonRemoteValidationModel
            {
                UserName = "Ann",
                FirstName = "Ann",
                LastName = "Beebe",
                //                Age = 43,
                City = "Boston"
            });
        }


        public List<PersonRemoteValidationModel> _usrList = new List<PersonRemoteValidationModel>();

        public void Update(PersonRemoteValidationModel umToUpdate)
        {

            foreach (PersonRemoteValidationModel um in _usrList)
            {
                if (um.UserName == umToUpdate.UserName)
                {
                    _usrList.Remove(um);
                    _usrList.Add(umToUpdate);
                    break;
                }
            }
        }
    }

}