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
    public class RoleController : BaseAdminController
    {
        AdminProvider AdminProvider = new AdminProvider();
        RoleProvider RoleProvider = new RoleProvider();
        RoleAccessProvider RoleAccessProvider = new RoleAccessProvider();

        [UserAuthorize(AccessModule.Admin, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Admin, "VIEW")]
        public ActionResult RoleList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = RoleProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new RoleVM
            {
                Id = v.Id,
                Name = v.Name
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Admin, "ADD")]
        public ActionResult Add()
        {
            RoleVM model = new RoleVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Admin, "ADD")]
        public ActionResult Add(RoleVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var RoleVO = new ROLE();
                    RoleVO.Name = Model.Name;

                    RoleVO.Status = STATUS_CODE.ACTIVE;

                    var result = RoleProvider.Add(RoleVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("Edit", "Role", new { Id = result });
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

        [UserAuthorize(AccessModule.Admin, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = RoleProvider.Get(Id);

            RoleVM model = new RoleVM()
            {
                Id = v.Id,
                Name = v.Name
            };

            var RoleAccesses = new List<RoleAccessVM>();
            foreach (var am in Enum.GetValues(typeof(AccessModule)).Cast<AccessModule>())
            {
                var access_result = RoleAccessProvider.Get(Id, am.ToString());

                var RoleAccess = new RoleAccessVM();
                RoleAccess.AccessModule = am.ToString();
                RoleAccess.Viewable = access_result == null ? false : (access_result.Viewable ?? false);
                RoleAccess.Addable = access_result == null ? false : (access_result.Addable ?? false);
                RoleAccess.Editable = access_result == null ? false : (access_result.Editable ?? false);
                RoleAccess.Deletable = access_result == null ? false : (access_result.Deletable ?? false);

                RoleAccesses.Add(RoleAccess);
            }
            ViewBag.RoleAccesses = RoleAccesses;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Admin, "EDIT")]
        public ActionResult Edit(RoleVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var RoleVO = RoleProvider.Get(Model.Id);
                    RoleVO.Name = Model.Name;

                    RoleProvider.Edit(RoleVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("Edit", "Role", new { Id = Model.Id });
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

        [HttpPost]
        [UserAuthorize(AccessModule.Admin, "EDIT")]
        public ActionResult UpdateRoleAccess(FormCollection form)
        {
            long RoleId = long.Parse(form["RoleId"]);
            try
            {
                var AdminId = (long)Session["ID"];

                foreach (var am in Enum.GetValues(typeof(AccessModule)).Cast<AccessModule>())
                {
                    ROLE_ACCESS RoleAccess = new ROLE_ACCESS();

                    RoleAccess.RoleId = RoleId;
                    RoleAccess.AccessModule = am.ToString();

                    RoleAccess.Viewable = form[am.ToString() + "-IsViewable"] == null ? false : true;
                    RoleAccess.Addable = form[am.ToString() + "-IsAddable"] == null ? false : true;
                    RoleAccess.Editable = form[am.ToString() + "-IsEditable"] == null ? false : true;
                    RoleAccess.Deletable = form[am.ToString() + "-IsDeleteable"] == null ? false : true;

                    RoleAccessProvider.SetAccess(RoleAccess, AdminId);
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("Edit", "Role", new { Id = RoleId });
        }

    }
}