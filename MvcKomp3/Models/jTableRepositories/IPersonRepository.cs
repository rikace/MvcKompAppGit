

using Hik.JTable.Models;
using System.Collections.Generic;
namespace Hik.JTable.Repositories
{
    public interface IPersonRepository
    {
        List<Person> GetAllPeople();
        Person AddPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(int personId);
    }
}
