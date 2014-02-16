using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Domain.Common;
using Entity;

namespace MvcNotes.Domain.Services
{
    public interface IPresidentService
    {
        IList<President> GetPresidents(Expression<Func<President, bool>> predicate);
        President GetPresident(int id);
    }

    public class PresidentService : IPresidentService
    {
        private readonly IRepository<President> _presidentsRepository;
        private readonly IUnitOfWork _uow;

        public PresidentService(IRepository<President> presidentRepository, IUnitOfWork uow)
        {
            _presidentsRepository = presidentRepository;
            _uow = uow;
        }

        public IList<President> GetPresidents(Expression<Func<President, bool>> predicate)
        {
            return _presidentsRepository.FindAll().Where(predicate).ToList();
        }

        public President GetPresident(int id)
        {
            President president = _presidentsRepository.FindById(id);
            return president;
        }
    }
}