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
using B21C.Helper;
using System.Drawing;
using B21C.Enums;

namespace B21C.Controllers
{
    public class BannerController : BaseAdminController
    {
        BannerProvider BannerProvider = new BannerProvider();

        [UserAuthorize(AccessModule.Banner, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Banner, "VIEW")]
        public ActionResult BannerList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            var List = BannerProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new BannerVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                URL = v.URL,
                Name = v.Name,
                ImageURL = v.ImageURL,
                Status = v.Status,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Banner, "ADD")]
        public ActionResult Add()
        {
            BannerVM model = new BannerVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Banner, "ADD")]
        public ActionResult Add(BannerVM Model, HttpPostedFileBase file, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var BannerVO = new BANNER();
                    BannerVO.Name = Model.Name;
                    BannerVO.URL = Model.URL;

                    BannerVO.Status = STATUS_CODE.ACTIVE;

                    //Image Uploading
                    if (file != null)
                    {
                        var OriginalImage = Image.FromStream(file.InputStream, true, true);
                        decimal destW = OriginalImage.Width;
                        decimal destH = OriginalImage.Height;
                        ImageHelper.CalculateSizeByRatio(144, 62, ref destW, ref destH);

                        var CroppedImage = ImageHelper.CropImage(Image.FromStream(file.InputStream, true, true), (int)destW, (int)destH, true);
                        var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var FilePath = FileHelper.SaveImage(CroppedImage, file.FileName, postfix);
                        BannerVO.ImageURL = FilePath;
                    }
                    var result = BannerProvider.Add(BannerVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "Banner");
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

        [UserAuthorize(AccessModule.Banner, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = BannerProvider.Get(Id);

            BannerVM model = new BannerVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Name = v.Name,
                URL = v.URL,
                ImageURL = v.ImageURL,
                Status = v.Status,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Banner, "EDIT")]
        public ActionResult Edit(BannerVM Model, HttpPostedFileBase file, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var BannerVO = BannerProvider.Get(Model.Id);
                    BannerVO.Name = Model.Name;
                    BannerVO.URL = Model.URL;

                    BannerVO.Status = STATUS_CODE.ACTIVE;

                    //Image Uploading
                    if (file != null)
                    {
                        var OriginalImage = Image.FromStream(file.InputStream, true, true);
                        decimal destW = OriginalImage.Width;
                        decimal destH = OriginalImage.Height;
                        ImageHelper.CalculateSizeByRatio(144, 62, ref destW, ref destH);

                        var CroppedImage = ImageHelper.CropImage(Image.FromStream(file.InputStream, true, true), (int)destW, (int)destH, true);
                        var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var FilePath = FileHelper.SaveImage(CroppedImage, file.FileName, postfix);
                        BannerVO.ImageURL = FilePath;
                    }

                    BannerProvider.Edit(BannerVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Banner");
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

        [UserAuthorize(AccessModule.Banner, "EDIT")]
        public ActionResult Publish(long Id)
        {
            var AdminId = (long)Session["ID"];

            var BannerVO = BannerProvider.Get(Id);
            BannerVO.Status = STATUS_CODE.PUBLISHED;

            BannerProvider.Edit(BannerVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Banner");
        }

        [UserAuthorize(AccessModule.Banner, "EDIT")]
        public ActionResult Hide(long Id)
        {
            var AdminId = (long)Session["ID"];

            var BannerVO = BannerProvider.Get(Id);
            BannerVO.Status = STATUS_CODE.ACTIVE;

            BannerProvider.Edit(BannerVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Banner");
        }

        [UserAuthorize(AccessModule.Banner, "DELETE")]
        public ActionResult Delete(long Id)
        {
            BannerProvider.Delete(Id);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Banner");
        }

    }
}