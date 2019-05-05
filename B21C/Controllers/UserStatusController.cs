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
using System.Drawing;
using B21C.Helper;

namespace B21C.Controllers
{
    public class UserStatusController : BaseAdminController
    {
        UserStatusProvider UserStatusProvider = new UserStatusProvider();

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult UserStatusList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = UserStatusProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new UserStatusVM
            {
                Id = v.Id,
                Name = v.Name,
                Discount = (int)v.Discount,
                PartnerDiscount = (int)(v.PartnerDiscount ?? 0),
                PointNeeded = v.PointNeeded,
                IconImage = v.IconImage,
                RegistrationFee = (int)(v.RegistrationFee ?? 0)
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Buyer, "ADD")]
        public ActionResult Add()
        {
            UserStatusVM model = new UserStatusVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Buyer, "ADD")]
        public ActionResult Add(UserStatusVM Model, HttpPostedFileBase file, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    if (UserStatusProvider.GetAll().Where(m => m.PointNeeded == Model.PointNeeded).Count() > 0)
                    {
                        throw new Exception("This Point Needed already exists.");
                    }

                    var UserStatusVO = new USER_STATUS();
                    UserStatusVO.Name = Model.Name;
                    UserStatusVO.Discount = Model.Discount;
                    UserStatusVO.PartnerDiscount = Model.PartnerDiscount;
                    UserStatusVO.PointNeeded = Model.PointNeeded;
                    UserStatusVO.RegistrationFee = Model.RegistrationFee;

                    //Image Uploading
                    if (file != null)
                    {
                        var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");

                        var OriginalImage = Image.FromStream(file.InputStream, true, true);
                        var FilePath = FileHelper.SaveImage(OriginalImage, file.FileName, postfix);

                        UserStatusVO.IconImage = FilePath;
                    }

                    var result = UserStatusProvider.Add(UserStatusVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "UserStatus");
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

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = UserStatusProvider.Get(Id);

            UserStatusVM model = new UserStatusVM()
            {
                Id = v.Id,
                Name = v.Name,
                Discount = (int)v.Discount,
                PartnerDiscount = (int)(v.PartnerDiscount ?? 0),
                PointNeeded = v.PointNeeded,
                IconImage = v.IconImage,
                RegistrationFee = (int)(v.RegistrationFee ?? 0)
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Edit(UserStatusVM Model, HttpPostedFileBase file, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    if (UserStatusProvider.GetAll().Where(m => m.PointNeeded == Model.PointNeeded && m.Id != Model.Id).Count() > 0)
                    {
                        throw new Exception("This Point Needed already exists.");
                    }

                    var UserStatusVO = UserStatusProvider.Get(Model.Id);
                    UserStatusVO.Name = Model.Name;
                    UserStatusVO.Discount = Model.Discount;
                    UserStatusVO.PartnerDiscount = Model.PartnerDiscount;
                    UserStatusVO.PointNeeded = Model.PointNeeded;
                    UserStatusVO.RegistrationFee = Model.RegistrationFee;

                    //Image Uploading
                    if (file != null)
                    {
                        var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");

                        var OriginalImage = Image.FromStream(file.InputStream, true, true);
                        var FilePath = FileHelper.SaveImage(OriginalImage, file.FileName, postfix);

                        UserStatusVO.IconImage = FilePath;
                    }

                    UserStatusProvider.Edit(UserStatusVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "UserStatus");
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