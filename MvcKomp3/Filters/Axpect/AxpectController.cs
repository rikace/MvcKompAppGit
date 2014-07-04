using System.Web.Mvc;

namespace BookSamples.Components.Filters.Axpect
{
    public class AxpectController : Controller
    {
        public AxpectController()
        {
            ActionInvoker = new AxpectActionInvoker();
        }
    }
}