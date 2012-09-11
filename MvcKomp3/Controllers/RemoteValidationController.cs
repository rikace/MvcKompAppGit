using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MvcKompApp.Framework;

namespace MvcKompApp.Controllers
{
    public class RemoteValidationController : Controller
    {
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]

        public string IsUID_Available(string candidate)
        {

            if (UserNameHelper.IsAvailable(candidate))
                return "OK";

            for (int i = 1; i < 10; i++)
            {
                string altCandidate = candidate + i.ToString();
                if (UserNameHelper.IsAvailable(altCandidate))
                    return String.Format(CultureInfo.InvariantCulture,
                   "{0} is not available. Try {1}.", candidate, altCandidate);
            }
            return String.Format(CultureInfo.InvariantCulture,
                "{0} is not available.", candidate);
        }

        public ActionResult Index()
        {
            return View(UsrLstContainer.getUsrLst());
        }

        public ActionResult Edit(string id)
        {
            return View(GetUser(id));
        }

        [HttpPost]
        public ActionResult Edit(MvcKompApp.Models.PersonRemoteValidationModel um)
        {
            if (!TryUpdateModel(um))
                return View(um);

            // ToDo: add persistent to DB.
            UsrLstContainer.getUsrLstContainer().Update(um);
            return View("Details", um);
        }

        public ViewResult Create()
        {
            return View(new MvcKompApp.Models.PersonRemoteValidationModel());
        }

        [HttpPost]
        public ActionResult Create(MvcKompApp.Models.PersonRemoteValidationModel um)
        {
            if (!UserNameHelper.IsAvailable(um.UserName))
            {

                return View(um);
            }
            else
            {
                if (!TryUpdateModel(um))
                    return View(um);

                // ToDo: add persistent to DB.
                UsrLstContainer.getUsrLst().Add(um);
                return View("Details", um);
            }
        }

        MvcKompApp.Models.PersonRemoteValidationModel GetUser(string uid)
        {
            MvcKompApp.Models.PersonRemoteValidationModel usrMdl = null;

            foreach (MvcKompApp.Models.PersonRemoteValidationModel um in UsrLstContainer.getUsrLst())
                if (um.UserName == uid)
                    usrMdl = um;

            return usrMdl;
        }

    }
}
