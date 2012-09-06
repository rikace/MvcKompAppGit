using System.Web.Mvc;
using MvcKompApp.Models;

namespace MvcKompApp.Infrastructure
{
    public class CartModelBinder:IModelBinder
    {
        private const string sessionKey = "Cart";


        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Cart cart = (Cart)controllerContext.HttpContext.Session[sessionKey];

            if (cart == null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[sessionKey] = cart;
            } 
            return cart;
        }

        #endregion
    }
}