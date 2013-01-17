using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class HttpApiController<T> : ApiController where T : class, IEntity
    {
        protected List<T> Source { get; private set; }

        public HttpApiController(List<T> source)
        {
            this.Source = source;
        }

        public IQueryable<T> Get()
        {
            return this.Source.AsQueryable();
        }

        public T Get(int id)
        {
            var entity = this.Source.SingleOrDefault(c => c.Id == id);
            if (entity == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return entity;
        }

        public HttpResponseMessage Post(T entity)
        {
            this.OnHtttpPostHandled(entity);
            this.Source.Add(entity);
            var response = this.Request.CreateResponse<T>(HttpStatusCode.Created, entity);
            string uri = this.Url.Link("DefaultApi", new { id = entity.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        protected virtual void OnHtttpPostHandled(T entity)
        {
        }

        public void Put(int id, T entity)
        {
            var existing = this.Source.Single(c => c.Id == entity.Id);
            if (existing == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            this.Source.Remove(existing);
            this.Source.Add(entity);
        }

        public HttpResponseMessage Delete(int id)
        {
            var found = this.Source.SingleOrDefault(c => c.Id == id);
            if (found != null)
            {
                this.Source.Remove(found);
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
    public interface IEntity
    {
        int Id { get; set; }
    }
}