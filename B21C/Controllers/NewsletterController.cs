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
    public class NewsletterController : BaseAdminController
    {
        NewsletterProvider NewsletterProvider = new NewsletterProvider();
        NewsletterSubscriberProvider NewsletterSubscriberProvider = new NewsletterSubscriberProvider();
        UserProvider UserProvider = new UserProvider();
        UserStatusProvider UserStatusProvider = new UserStatusProvider();

        [UserAuthorize(AccessModule.Newsletter, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Newsletter, "VIEW")]
        public ActionResult NewsletterList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = NewsletterProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new NewsletterVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Subject = v.Subject,
                Body = v.Body,
                Status = v.Status,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Newsletter, "ADD")]
        public ActionResult Add()
        {
            NewsletterVM model = new NewsletterVM();

            return View(model);
        }

        [UserAuthorize(AccessModule.Newsletter, "ADD")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(NewsletterVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var NewsletterVO = new EMAIL();
                    NewsletterVO.Subject = Model.Subject;
                    NewsletterVO.Body = Model.Body;

                    NewsletterVO.Status = STATUS_CODE.ACTIVE;

                    var result = NewsletterProvider.Add(NewsletterVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "Newsletter");
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
            var v = NewsletterProvider.Get(Id);

            NewsletterVM model = new NewsletterVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Subject = v.Subject,
                Body = v.Body,
                Status = v.Status,
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [UserAuthorize(AccessModule.Newsletter, "EDIT")]
        public ActionResult Edit(NewsletterVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var NewsletterVO = NewsletterProvider.Get(Model.Id);
                    NewsletterVO.Subject = Model.Subject;
                    NewsletterVO.Body = Model.Body;

                    NewsletterProvider.Edit(NewsletterVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Newsletter");
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
        public ActionResult Send(long Id)
        {
            SendNewsletterVM model = new SendNewsletterVM()
            {
                Id = Id,
            };

            var TargetList = new List<SelectListItem>();
            foreach (var us in UserStatusProvider.GetAll())
            {
                TargetList.Add(new SelectListItem() { Value = us.Id.ToString(), Text = us.Name });
            }
            TargetList.Add(new SelectListItem() { Value = "Subscriber", Text = "Subscriber" });
            ViewBag.TargetList = TargetList;

            return View(model);
        }

        [UserAuthorize(AccessModule.Newsletter, "EDIT")]
        [HttpPost]
        public ActionResult Send(SendNewsletterVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Newsletter = NewsletterProvider.Get(Model.Id);

                    List<string> TargetEmailList = new List<string>();

                    foreach (var t in Model.Target)
                    {
                        if (t == "Subscriber")
                        {
                            TargetEmailList = TargetEmailList.Union(NewsletterSubscriberProvider.GetAll().Where(m => m.Status == STATUS_CODE.ACTIVE).Select(m => m.Email)).ToList();
                        }
                        else
                        {
                            try
                            {
                                var UserStatusId = long.Parse(t);

                                TargetEmailList = TargetEmailList.Union(UserProvider.GetAllBuyerByWebsite("b21c").Where(m => m.UserStatusId == UserStatusId).Select(m => m.Username)).ToList();
                            }
                            catch { }
                        }
                    }

                    TargetEmailList = TargetEmailList.Select(m => m.ToLower()).Distinct().ToList();

                    foreach (var v in TargetEmailList)
                    {
                        try
                        {
                            Helper.Email.SendGrid(v, Newsletter.Subject, Newsletter.Body);
                        }
                        catch { }
                    }

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Newsletter");
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

            return RedirectToAction("Send", "Newsletter", new { Id = Model.Id });
        }

    }
}