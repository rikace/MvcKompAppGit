using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class CustomerGridController : ApiController
    {
        ICustomerRepository _contacts = new CustomerRepository();

        // Get All Contacts
        public IEnumerable<Customer> Get()
        {
            return _contacts.GetAllCustomers();
        }

        // Get Contact by Id
        public Customer Get(int id)
        {
            try
            {
                Customer contact = _contacts.GetCustomer(id);
                return contact;
            }
            catch (Exception ex)
            {
                 throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, ex.Message));                
            }
        }
        
        // Insert Contact
        public HttpResponseMessage Post(Customer value)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Customer contact = _contacts.AddCustomer(value);
                    var response = Request.CreateResponse<Customer>(HttpStatusCode.Created, contact);
                    return response;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Model state is invalid");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        // Update Contacts
        public HttpResponseMessage Put(int id, Customer value)
        {
            try
            {
                value.Id = id;
                _contacts.UpdateCustomer(id, value);              
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // Delete Contacts
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                _contacts.RemoveCustomer(id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
