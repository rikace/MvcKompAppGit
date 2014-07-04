using System;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class TrackActionAttribute : ActionFilterAttribute
    {
        public String FileName { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (String.IsNullOrEmpty(FileName))
                throw new ArgumentNullException();

            // This is not really real code but just helps you form a reasonably clear idea of it...
            var file = new FileStream(FileName, FileMode.Append, FileAccess.Write);
            if (file.CanWrite)
            {
                var text = String.Format("{0}.{1} executed @ {2}", 
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName,
                    DateTime.Now);
                var encoding = new UTF8Encoding();
                file.Write(encoding.GetBytes(text), 0, text.Length);
            }
                
            file.Close();
            return;
        }

    }
}