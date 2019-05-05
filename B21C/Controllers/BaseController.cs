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
    public class BaseController : Controller
    {
        protected override void ExecuteCore()
        {
            var GoodToGo = true;
            if (((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Reseller").Count() == 0)
            {
                GoodToGo = false;
            }
            else
            {
                if (User.Identity.IsAuthenticated == true && GoodToGo == true)
                {
                    Session["User_ID"] = User.Identity.GetUserId<long>();
                    Session["User_Username"] = User.Identity.Name;

                    if (Session["User_Username"] != null && Session["User_ID"] != null)
                    {
                        var UncompletedSurvey = new SurveyProvider().GetUncompletedSurveyByUser((long)Session["User_ID"]).FirstOrDefault();

                        if (UncompletedSurvey != null)
                        {
                            Session["User_SurveyID"] = UncompletedSurvey.Id;
                            Session["User_SurveyURL"] = UncompletedSurvey.URL;
                        }
                    }
                }
            }
            base.ExecuteCore();
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

    }
}