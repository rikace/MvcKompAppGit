using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVCNestedModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MVCControlsToolkit.Controller;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.ActionFilters;
using System.Linq.Expressions;

namespace MVCNestedModels.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************
        public void CreatePages(int numPages, int itemsPerPage, ref List<Tracker<ToDoItem>>[] AllPages)
        {
            AllPages = new List<Tracker<ToDoItem>>[numPages];
            int code =1;
            for (int i = 0; i < numPages; i++ )
            {
                List<Tracker<ToDoItem>> currPage = new List<Tracker<ToDoItem>>();
                for (int j = 1; j <= itemsPerPage; j++)
                {
                    currPage.Add(new Tracker<ToDoItem>(new ToDoItemExt()
                    {
                        Code = code,
                        Name = string.Format("name item {0} page {1}", j, i+1),
                        Description = string.Format("a) description item {0} page {1}", j, i+1),
                        Important = true,
                        ToDoRoles = new List<int> { 1, 2 }
                    }));
                    code++;
                    currPage.Add(new Tracker<ToDoItem>(new ToDoItem()
                    {
                        Code = code,
                        Name = string.Format("name item {0} page {1}", j, i + 1),
                        Description = string.Format("b) description item {0} page {1}", j, i + 1),
                        Important = false
                    }));
                    code++;
                }
                AllPages[i] = currPage;
            }
        }
        
        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            RegisterModel rm = new RegisterModel() 
            { 
                Keywords = new string[] { "francesco", "Abbruzzese", "Programmer" },
                Keywords1 = new List<KeywordItem> { 
                    new KeywordItem{Keyword="francesco", Title="My Program 1"},
                    new KeywordItemExt{Keyword="Giovanni", Title="My Program 2"},
                    new KeywordItem{Keyword="Fausto", Title="My Program 3"}
                }, 
                UserName = "francesco", Roles = new List<int> { 4 } };
            rm.Start = new DateTime(2010, 3, 2, 16, 20, 22);
            rm.Stop = new DateTime(2010, 3, 2, 20, 20, 22);
            rm.Delay = new TimeSpan(0, -4, 0, 0, 0);
 
            
            rm.Page = 1;
            rm.PrevPage = rm.Page;
            List<Tracker<ToDoItem>>[] AllPages=null;
            rm.TotalPages = 10; CreatePages(rm.TotalPages, 4, ref AllPages);
            Session["AllPages"] = AllPages;
            if (rm.Page > rm.TotalPages) rm.Page = rm.TotalPages;
            rm.ToDoList = AllPages[rm.Page - 1];
            
            rm.Selection = "choice2";
            rm.PersonalData = new PersonalInfosExt();
            rm.PersonalData.MinAge = 18f;

            rm.PersonalData.MaxAge = 40f;
            rm.PersonalData.MinAgeDelay=0f;

            rm.ClientKeywords = new List<string>();
            rm.ClientKeywords.Add("To Start");
            
            
            rm.EmailFolders= new List<EmailElement>();
            EmailElement friends = new EmailFolder()
            {
                Name = "Friends",
                Children =
                    new List<EmailElement>{
                            new EmailDocument{
                                Name="EMail_Friend_1"
                            },
                            new EmailDocument{
                                Name="EMail_Friend_2"
                            },
                            new EmailDocument{
                                Name="EMail_Friend_3"
                            }
                        }

            };
            rm.EmailFolders.Add(friends);

            EmailElement relatives = new EmailFolder()
            {
                Name = "Relatives",
                Children =
                    new List<EmailElement>{
                            new EmailDocument{
                                Name="EMail_Relatives_1"
                            },
                            new EmailDocument{
                                Name="EMail_Relatives_2"
                            },
                            new EmailDocument{
                                Name="EMail_Relatives_3"
                            }
                        }

            };
            rm.EmailFolders.Add(relatives);
            EmailElement work = new EmailFolder()
            {
                Name = "Work",
                Children =
                    new List<EmailElement>{
                            new EmailDocument{
                                Name="EMail_Work_1"
                            },
                            new EmailDocument{
                                Name="EMail_Work2"
                            },
                            new EmailDocument{
                                Name="EMail_Work_3"
                            }
                        }

            };
            rm.EmailFolders.Add(work);
            rm.EmailFolders = new List<EmailElement>
            {
                new EmailFolder{
                    Name = "All Folders",
                    Children = rm.EmailFolders
                }
            };
            return View(rm);
            

        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
           
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }

            // If we got this far, something failed, redisplay form
            model.Delay = new TimeSpan(0, -4, 0, 0, 0);
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (model.Page < 1) model.Page = 1;
            if (model.Page > 10) model.Page = 10;
            model.TotalPages = 10;
            List<Tracker<ToDoItem>>[] AllPages = Session["AllPages"] as List<Tracker<ToDoItem>>[];
            
            if (model.ToDoOrder != null && model.ToDoOrder.Count > 0)
            {
                var orderedItems = (from x in model.ToDoList where x.Value != null select x.Value).ApplyOrder(model.ToDoOrder);
                model.ToDoList = (from y in orderedItems select new Tracker<ToDoItem>() { Value = y, OldValue = y }).ToList(); 
            }

            if (model.KeywordsOrdering != null && model.KeywordsOrdering.Count > 0)
            {
                model.Keywords1 = model.Keywords1.ApplyOrder(model.KeywordsOrdering).ToList();
            }
            AllPages[model.PrevPage - 1] = model.ToDoList;
            model.ToDoList = AllPages[model.Page - 1];
            Session["AllPages"] = AllPages;
            model.PrevPage = model.Page;
            return View(model);
        }
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] 
        public ActionResult ServerTime()
        {
            DateTime now = DateTime.Now;
            return PartialView("ServerTime", now);
        }
        private Tracker<ToDoItem> FindTrackerToDo(int id)
        {
            List<Tracker<ToDoItem>>[] AllPages = Session["AllPages"] as List<Tracker<ToDoItem>>[];
            if (AllPages == null)
            {
                CreatePages(10, 4, ref AllPages);
                Session["AllPages"] = AllPages;
            }
            for (int i = 0; i < AllPages.Length; i++)
            {
                foreach (Tracker<ToDoItem> t in AllPages[i])
                {
                    if (t.OldValue.Code == id) 
                        return t;
                }
            }
            return null;
        }

        public ActionResult AvailableRoles()
        {
            var res = Json(MVCControlsToolkit.Controls.ChoiceListHelper.CreateGrouped(RegisterModel.AllRoles,
                                m => m.Code,
                                m => m.Name,
                                m => m.GroupCode,
                                m => m.GroupName).PrepareForJson(), JsonRequestBehavior.AllowGet);
            return res;
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] 
        public ActionResult EditDetailToDo(int id)
        {
            
            if (id == 0) return new MVCControlsToolkit.Controller.ClentValidationViewResult("EditDetailToDo", new ToDoItem());

            Tracker<ToDoItem> t = FindTrackerToDo(id);
            if (t != null) return new MVCControlsToolkit.Controller.ClentValidationViewResult("EditDetailToDo", t.Value);
            return new MVCControlsToolkit.Controller.ClentValidationViewResult("EditDetailToDo", new ToDoItem());
        }

       
        [HttpPost]
        public ActionResult EditDetailToDo(ToDoItem item)
        { 
           
            List<Tracker<ToDoItem>>[] AllPages = Session["AllPages"] as List<Tracker<ToDoItem>>[];
            if (AllPages == null)
            {
                CreatePages(10, 4, ref AllPages);
                Session["AllPages"] = AllPages;
            }
            if (item.Code == 0)
            {
                item.Code = AllPages[AllPages.Length - 1].Last().OldValue.Code + 1;
                AllPages[AllPages.Length - 1].Add(new Tracker<ToDoItem>(item));
            }
            else
            {
                
                Tracker<ToDoItem> t = FindTrackerToDo(item.Code);
                t.OldValue = item;
                t.Value = item;
            }


            return new MVCControlsToolkit.Controller.ClentValidationViewResult("EditDetailToDo", item);
        }
        public ActionResult DisplayDetailToDo(int Code)
        {
            if (Code == 0) return PartialView(new ToDoItem());

            Tracker<ToDoItem> t = FindTrackerToDo(Code);
            if (t != null) return PartialView(t.OldValue);
            return PartialView(new ToDoItem());
        }
        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
