using B21C.Enums;
using B21C.Lib.Provider;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace B21C.Controllers
{
    public class BaseAdminController : Controller
    {
        protected override void ExecuteCore()
        {
            var GoodToGo = true;
            if (((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin").Count() == 0)
            {
                GoodToGo = false;
                Session.RemoveAll();
                HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie", "ExternalCookie");
            }

            if (Request.Path.ToLower().Contains("/admin/login"))
            {
                if (User.Identity.IsAuthenticated == true && GoodToGo == true)
                {
                    Session["ID"] = User.Identity.GetUserId<long>();
                    Session["Username"] = User.Identity.Name;

                    base.RedirectToAction("Index", "Admin").ExecuteResult(ControllerContext);
                }
                else
                {
                    base.ExecuteCore();
                }
            }
            else if (Request.Path.ToLower().Contains("/admin/logout"))
            {
                base.ExecuteCore();
            }
            else
            {
                if (User.Identity.IsAuthenticated == true && GoodToGo == true)
                {
                    Session["ID"] = User.Identity.GetUserId<long>();
                    Session["Username"] = User.Identity.Name;
                    if (Session["Username"] != null && Session["ID"] != null
                        && AdminProvider.IsAuthenticated((long)Session["ID"], (string)Session["Username"], "b21c")
                    )
                    {
                        base.ExecuteCore();
                    }
                }
                else
                {
                    ViewBag.ReturnURL = Server.UrlEncode(Request.RawUrl);
                    View("Admin_Unauthenticated").ExecuteResult(ControllerContext);
                }
            }
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    Exception ex = filterContext.Exception;
        //    filterContext.ExceptionHandled = true;

        //    var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

        //    filterContext.Result = new ViewResult()
        //    {
        //        ViewName = "Admin_Error",
        //        ViewData = new ViewDataDictionary(model)
        //    };

        //}

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        public class UserAuthorizeAttribute : AuthorizeAttribute
        {
            private readonly string Module;
            private readonly string Access;

            public UserAuthorizeAttribute(AccessModule Module, string Access)
            {
                this.Module = Module.ToString();
                this.Access = Access;
            }
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool authorize = false;

                var AdminId = (long)httpContext.Session["ID"];
                var Admin = new AdminProvider().Get(AdminId);
                var RoleAccess = new RoleAccessProvider().Get(Admin.RoleId ?? 0, Module);

                if (RoleAccess != null)
                {
                    switch (Access)
                    {
                        case "VIEW":
                            authorize = RoleAccess.Viewable ?? false;
                            break;
                        case "ADD":
                            authorize = RoleAccess.Addable ?? false;
                            break;
                        case "EDIT":
                            authorize = RoleAccess.Editable ?? false;
                            break;
                        case "DELETE":
                            authorize = RoleAccess.Deletable ?? false;
                            break;
                    }
                }

                return authorize;
            }
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                filterContext.Result = new ViewResult { ViewName = "Admin_Unauthorized" };
            }
        }  

    }
}