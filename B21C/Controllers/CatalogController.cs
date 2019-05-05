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
    public class CatalogController : BaseAdminController
    {
        CatalogProvider CatalogProvider = new CatalogProvider();
        NewsletterSubscriberProvider NewsletterSubscriberProvider = new NewsletterSubscriberProvider();

        [UserAuthorize(AccessModule.Catalog, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Catalog, "VIEW")]
        public ActionResult CatalogList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = CatalogProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new CatalogVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                URL = v.URL,
                Name = v.Name,
                Status = v.Status,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Catalog, "ADD")]
        public ActionResult Add()
        {
            CatalogVM model = new CatalogVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Catalog, "ADD")]
        public ActionResult Add(CatalogVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var CatalogVO = new CATALOG();
                    CatalogVO.Name = Model.Name;
                    CatalogVO.URL = Model.URL;

                    CatalogVO.Status = STATUS_CODE.ACTIVE;

                    var result = CatalogProvider.Add(CatalogVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "Catalog");
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

        [UserAuthorize(AccessModule.Catalog, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = CatalogProvider.Get(Id);

            CatalogVM model = new CatalogVM()
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
        [UserAuthorize(AccessModule.Catalog, "EDIT")]
        public ActionResult Edit(CatalogVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var CatalogVO = CatalogProvider.Get(Model.Id);
                    CatalogVO.Name = Model.Name;
                    CatalogVO.URL = Model.URL;

                    CatalogProvider.Edit(CatalogVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Catalog");
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

    }
}