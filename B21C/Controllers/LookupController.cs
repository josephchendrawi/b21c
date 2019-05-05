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
    public class LookupController : BaseAdminController
    {
        LookupProvider LookupProvider = new LookupProvider();
        UserProvider UserProvider = new UserProvider();
        UserStatusProvider UserStatusProvider = new UserStatusProvider();

        [UserAuthorize(AccessModule.Lookup, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Lookup, "VIEW")]
        public ActionResult LookupList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = LookupProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new LookupVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Name = v.Name,
                Type = v.Type,
                Value = v.Value
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Lookup, "ADD")]
        public ActionResult Add()
        {
            LookupVM model = new LookupVM();

            return View(model);
        }

        [UserAuthorize(AccessModule.Lookup, "ADD")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(LookupVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var LookupVO = new LOOKUP();
                    LookupVO.Name = Model.Name;
                    LookupVO.Type = Model.Type;
                    LookupVO.Value = Model.Value;

                    LookupVO.ActiveFlag = true;

                    var result = LookupProvider.Add(LookupVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "Lookup");
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

        [UserAuthorize(AccessModule.Lookup, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = LookupProvider.Get(Id);

            LookupVM model = new LookupVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Name = v.Name,
                Type = v.Type,
                Value = v.Value
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [UserAuthorize(AccessModule.Lookup, "EDIT")]
        public ActionResult Edit(LookupVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var LookupVO = LookupProvider.Get(Model.Id);
                    LookupVO.Name = Model.Name;
                    LookupVO.Value = Model.Value;

                    LookupProvider.Edit(LookupVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Lookup");
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