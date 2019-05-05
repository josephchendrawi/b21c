using B21C.Lib.Provider;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B21C.Models;
using B21C.EF;
using B21C.Lib.Common.Constants;
using B21C.Enums;

namespace B21C.Controllers
{
    public class NewsletterSubscriberController : BaseAdminController
    {
        NewsletterSubscriberProvider NewsletterSubscriberProvider = new NewsletterSubscriberProvider();

        [UserAuthorize(AccessModule.Newsletter, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Newsletter, "VIEW")]
        public ActionResult NewsletterSubscriberList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = NewsletterSubscriberProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new NewsletterSubscriberVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Email = v.Email,
                Status = v.Status,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Newsletter, "ADD")]
        public ActionResult Add()
        {
            NewsletterSubscriberVM model = new NewsletterSubscriberVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Newsletter, "ADD")]
        public ActionResult Add(NewsletterSubscriberVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var NewsletterSubscriberVO = new EMAIL_SUBSCRIBER();
                    NewsletterSubscriberVO.Email = Model.Email;

                    NewsletterSubscriberVO.Status = STATUS_CODE.ACTIVE;

                    var result = NewsletterSubscriberProvider.Add(NewsletterSubscriberVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "NewsletterSubscriber");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = string.Join(" ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return View(Model);
        }

        [UserAuthorize(AccessModule.Newsletter, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = NewsletterSubscriberProvider.Get(Id);

            NewsletterSubscriberVM model = new NewsletterSubscriberVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Email = v.Email,
                Status = v.Status,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Newsletter, "EDIT")]
        public ActionResult Edit(NewsletterSubscriberVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var NewsletterSubscriberVO = NewsletterSubscriberProvider.Get(Model.Id);
                    NewsletterSubscriberVO.Email = Model.Email;

                    NewsletterSubscriberProvider.Edit(NewsletterSubscriberVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "NewsletterSubscriber");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = string.Join(" ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return View(Model);
        }

        [UserAuthorize(AccessModule.Newsletter, "EDIT")]
        public ActionResult Toggle(long Id)
        {
            var AdminId = (long)Session["ID"];

            var v = NewsletterSubscriberProvider.Get(Id);
            v.Status = v.Status == STATUS_CODE.INACTIVE ? STATUS_CODE.ACTIVE : STATUS_CODE.INACTIVE;

            NewsletterSubscriberProvider.Edit(v, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "NewsletterSubscriber");
        }

    }
}