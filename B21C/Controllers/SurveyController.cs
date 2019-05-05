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
    public class SurveyController : BaseAdminController
    {
        SurveyProvider SurveyProvider = new SurveyProvider();
        NewsletterSubscriberProvider NewsletterSubscriberProvider = new NewsletterSubscriberProvider();

        [UserAuthorize(AccessModule.Survey, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Survey, "VIEW")]
        public ActionResult SurveyList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = SurveyProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new SurveyVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                URL = v.URL,
                Name = v.Name,
                Status = v.Status,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Survey, "ADD")]
        public ActionResult Add()
        {
            SurveyVM model = new SurveyVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Survey, "ADD")]
        public ActionResult Add(SurveyVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var SurveyVO = new SURVEY();
                    SurveyVO.Name = Model.Name;
                    SurveyVO.URL = Model.URL;

                    SurveyVO.Status = STATUS_CODE.INACTIVE;

                    var result = SurveyProvider.Add(SurveyVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "Survey");
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

        [UserAuthorize(AccessModule.Survey, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = SurveyProvider.Get(Id);

            SurveyVM model = new SurveyVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Name = v.Name,
                URL = v.URL,
                Status = v.Status,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Survey, "EDIT")]
        public ActionResult Edit(SurveyVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var SurveyVO = SurveyProvider.Get(Model.Id);
                    SurveyVO.Name = Model.Name;
                    SurveyVO.URL = Model.URL;

                    SurveyProvider.Edit(SurveyVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Survey");
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

        [UserAuthorize(AccessModule.Survey, "EDIT")]
        public ActionResult Publish(long Id)
        {
            var AdminId = (long)Session["ID"];

            var SurveyVO = SurveyProvider.Get(Id);
            SurveyVO.Status = STATUS_CODE.ACTIVE;

            SurveyProvider.Edit(SurveyVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Survey");
        }

        [UserAuthorize(AccessModule.Survey, "EDIT")]
        public ActionResult Stop(long Id)
        {
            var AdminId = (long)Session["ID"];

            var SurveyVO = SurveyProvider.Get(Id);
            SurveyVO.Status = STATUS_CODE.INACTIVE;

            SurveyProvider.Edit(SurveyVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Survey");
        }

        [UserAuthorize(AccessModule.Survey, "DELETE")]
        public ActionResult Delete(long Id)
        {
            SurveyProvider.Delete(Id);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Survey");
        }

    }
}