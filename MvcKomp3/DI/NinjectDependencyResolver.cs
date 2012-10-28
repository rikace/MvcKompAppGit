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

    public class NinjectDependencyKernelResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyKernelResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public IBindingToSyntax<T> Bind<T>()
        {
            return kernel.Bind<T>();
        }

        public IKernel Kernel
        {
            get { return kernel; }
        }

        private void AddBindings()
        {
            // put additional bindings here
            // Bind<IMessageProvider>().To<SimpleMessageProvider>();
        }
    }
}