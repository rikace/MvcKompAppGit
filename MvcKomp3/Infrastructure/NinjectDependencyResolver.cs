using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Ninject.Syntax;

namespace MvcKomp3.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IResolutionRoot _kernel;

        public NinjectDependencyResolver()
            : this(new StandardKernel())
        { }

        public NinjectDependencyResolver(IResolutionRoot kernel)
        {
            _kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
}