using System.Collections.Generic;
using Entities;
using MvcKompApp.Infrastructure;

namespace MvcKompApp.Models
{
    public class EmployeeWithPaging
    {
        public List<Employee> Employees { get; set; }
        public PagingInfo Paging { get; set; }
    }
}