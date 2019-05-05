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
    public class ProductCategoryController : BaseAdminController
    {
        ProductCategoryProvider ProductCategoryProvider = new ProductCategoryProvider();

        [NonAction]
        public string GetFullPathName(long CategoryId)
        {
            string result = "";

            if (CategoryId != 0)
            {
                var Category = ProductCategoryProvider.Get(CategoryId);

                if (Category != null)
                {
                    var ParentCategoryName = GetFullPathName(Category.ParentCategoryId ?? 0);

                    if (string.IsNullOrWhiteSpace(ParentCategoryName))
                        result = Category.Name;
                    else
                        result = ParentCategoryName + " > " + Category.Name;
                }
            }

            return result;
        }

        [UserAuthorize(AccessModule.ProductCategory, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.ProductCategory, "VIEW")]
        public ActionResult ProductCategoryList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            request.RenameRequestFilterSortMember("ParentCategoryName", "ParentCategoryId");

            var List = ProductCategoryProvider.GetAll();

            DataSourceResult result = List.ToDataSourceResult(request, v => new ProductCategoryVM
            {
                Id = v.Id,
                Name = v.Name, //display also parent category name
                ParentCategoryId = v.ParentCategoryId,

                ParentCategoryName = GetFullPathName(v.ParentCategoryId ?? 0)
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.ProductCategory, "VIEW")]
        public JsonResult ProductCategoryList_DDL(long ExcludeCategoryId = 0)
        {
            var List = ProductCategoryProvider.GetAll().ToList();

            if (ExcludeCategoryId != 0)
                List = List.Where(m => m.Id != ExcludeCategoryId).ToList();

            List.Insert(0, new PRODUCT_CATEGORY() { Id = 0, Name = "-" });

            return Json(List.Select(m => new { CategoryId = m.Id, FullPathName = (m.Id == 0 ? m.Name : GetFullPathName(m.Id)) }).OrderBy(m => m.FullPathName), JsonRequestBehavior.AllowGet);
        }

        [UserAuthorize(AccessModule.ProductCategory, "ADD")]
        public ActionResult Add()
        {
            ProductCategoryVM model = new ProductCategoryVM();

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.ProductCategory, "ADD")]
        public ActionResult Add(ProductCategoryVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var ProductCategoryVO = new PRODUCT_CATEGORY();
                    ProductCategoryVO.Name = Model.Name;
                    ProductCategoryVO.ParentCategoryId = Model.ParentCategoryId;
                    
                    var result = ProductCategoryProvider.Add(ProductCategoryVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "ProductCategory");
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

        [UserAuthorize(AccessModule.ProductCategory, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = ProductCategoryProvider.Get(Id);

            ProductCategoryVM model = new ProductCategoryVM()
            {
                Id = v.Id,
                Name = v.Name,
                ParentCategoryId = v.ParentCategoryId
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.ProductCategory, "EDIT")]
        public ActionResult Edit(ProductCategoryVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var ProductCategoryVO = ProductCategoryProvider.Get(Model.Id);
                    ProductCategoryVO.Name = Model.Name;
                    ProductCategoryVO.ParentCategoryId = Model.ParentCategoryId;

                    ProductCategoryProvider.Edit(ProductCategoryVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "ProductCategory");
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