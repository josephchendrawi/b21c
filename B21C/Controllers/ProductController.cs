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
using B21C.Helper.AccessControl;

namespace B21C.Controllers
{
    public class ProductController : BaseAdminController
    {
        ProductProvider ProductProvider = new ProductProvider();
        ProductImageProvider ProductImageProvider = new ProductImageProvider();
        OrderProductProvider OrderProductProvider = new OrderProductProvider();
        ProductNewProvider ProductNewProvider = new ProductNewProvider();
        ProductStockActivityProvider ProductStockActivityProvider = new ProductStockActivityProvider();
        ProductOrderStatusResetProvider ProductOrderStatusResetProvider = new ProductOrderStatusResetProvider();

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult ProductList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            request.RenameRequestFilterSortMember("CategoryName", "PRODUCT_CATEGORY.Name");

            var List = ProductProvider.GetAll_DTO();

            DataSourceResult result = List.ToDataSourceResult(request, v => new ProductVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Description = v.Description,
                Name = v.Name,
                ModalPrice = (int)(v.ModalPrice ?? 0),
                Price = (int)(v.Price ?? 0),
                ProductCode = v.ProductCode,
                Size = v.Size,
                Stock = v.Stock,
                OldStock = v.OldStock,
                Weight = v.Weight,
                Sales = v.Sales ?? false,
                SalesDiscount = v.SalesDiscount,
                Point = v.Point ?? 0,
                TotalPrice = (int)((v.Price ?? 0) - (v.Sales == true ? (v.SalesDiscount ?? 0) : 0))
                        - (AccessControl.IsAccessable((long)Session["Id"], AccessModule.Order_ProductDisc35000) ? (v.Sales == true ? 0 : 35000) : 0),
                MainProductImage = (ProductImageProvider.GetMainImageByProductId(v.Id) ?? new PRODUCT_IMAGE()).URL,
                isNew = v.isNew,

                CategoryId = v.CategoryId ?? 0,
                CategoryName = v.PRODUCT_CATEGORY == null ? "" : v.PRODUCT_CATEGORY.Name,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Product, "ADD")]
        public ActionResult Add()
        {
            ProductVM model = new ProductVM();
            model.PublishAt = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Product, "ADD")]
        public ActionResult Add(ProductVM Model, HttpPostedFileBase[] file, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var ProductVO = new PRODUCT();
                    ProductVO.Name = Model.Name;
                    ProductVO.Description = Model.Description;
                    ProductVO.Name = Model.Name;
                    ProductVO.Price = Model.Price;
                    ProductVO.ModalPrice = Model.ModalPrice;
                    ProductVO.ProductCode = Model.ProductCode;
                    ProductVO.Size = Model.Size;
                    ProductVO.Stock = Model.Stock;
                    ProductVO.OldStock = 0;
                    ProductVO.Weight = Model.Weight;
                    ProductVO.Sales = Model.Sales;
                    ProductVO.SalesDiscount = Model.SalesDiscount;
                    ProductVO.Point = Model.Point;
                    ProductVO.PublishAt = Model.PublishAt;
                    ProductVO.Section = !Model.SpecialPromo ? ProductVO.Section : PRODUCT_SECTION.SPECIAL_PROMO + "|";
                    ProductVO.CategoryId = Model.CategoryId;

                    ProductVO.Status = STATUS_CODE.ACTIVE;

                    var result = ProductProvider.Add(ProductVO, AdminId);

                    if (result != 0)
                    {
                        //Image Uploading
                        if (file != null && file.Count() > 0)
                        {
                            List<string> filepaths = new List<string>();
                            foreach (var f in file)
                            {
                                if (f != null)
                                {
                                    var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");

                                    var OriginalImage = Image.FromStream(f.InputStream, true, true);
                                    var FilePath = FileHelper.SaveImage(OriginalImage, f.FileName, postfix);

                                    var size = Math.Min(OriginalImage.Width, OriginalImage.Height);
                                    var ThumbnailImage = ImageHelper.CropImage(OriginalImage, size, size, true);
                                    ThumbnailImage = ImageHelper.ResizeImage(ThumbnailImage, 300, 300);
                                    FileHelper.SaveImage(ThumbnailImage, "thumb_" + f.FileName, postfix);

                                    filepaths.Add(FilePath);
                                }
                            }

                            int DisplayOrder = 1;
                            foreach (var filepath in filepaths)
                            {
                                ProductImageProvider.Add(new PRODUCT_IMAGE()
                                {
                                    ProductId = result,
                                    URL = filepath,
                                    DisplayOrder = DisplayOrder,
                                }, AdminId);

                                DisplayOrder++;
                            }
                        }

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("ArrangeImage", "Product", new { Id = result });
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

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = ProductProvider.Get(Id);

            ProductVM model = new ProductVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Description = v.Description,
                Name = v.Name,
                Price = (int)(v.Price ?? 0),
                ModalPrice = (int)(v.ModalPrice ?? 0),
                ProductCode = v.ProductCode,
                Size = v.Size,
                Stock = v.Stock,
                OldStock = v.OldStock,
                Weight = v.Weight,
                Sales = v.Sales ?? false,
                SalesDiscount = (int)(v.SalesDiscount ?? 0),
                Point = v.Point ?? 0,
                PublishAt = v.PublishAt ?? DateTime.MinValue,
                SpecialPromo = v.Section == null ? false : v.Section.Contains(PRODUCT_SECTION.SPECIAL_PROMO),

                ProductImages = ProductImageProvider.GetAllByProductId(v.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList(),
                CategoryId = v.CategoryId ?? 0,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Edit(ProductVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var ProductVO = ProductProvider.Get(Model.Id);
                    ProductVO.Name = Model.Name;
                    ProductVO.Description = Model.Description;
                    ProductVO.Name = Model.Name;
                    ProductVO.Price = Model.Price;
                    ProductVO.ModalPrice = Model.ModalPrice;
                    ProductVO.ProductCode = Model.ProductCode;
                    ProductVO.Size = Model.Size;
                    ProductVO.OldStock = Model.OldStock;
                    ProductVO.Weight = Model.Weight;
                    ProductVO.Sales = Model.Sales;
                    ProductVO.SalesDiscount = Model.SalesDiscount;
                    ProductVO.Point = Model.Point;
                    ProductVO.Section = !Model.SpecialPromo ? (ProductVO.Section == null ? "" : ProductVO.Section.Replace(PRODUCT_SECTION.SPECIAL_PROMO + "|", "")) : (ProductVO.Section == null ? PRODUCT_SECTION.SPECIAL_PROMO + "|" : ProductVO.Section.Replace(PRODUCT_SECTION.SPECIAL_PROMO + "|", "") + PRODUCT_SECTION.SPECIAL_PROMO + "|");
                    ProductVO.CategoryId = Model.CategoryId;
                    ProductVO.PublishAt = Model.PublishAt;

                    ProductProvider.Edit(ProductVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("Edit", "Product", new { Id = Model.Id });
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

            Model.ProductImages = ProductImageProvider.GetAllByProductId(Model.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList();
            return View(Model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Edit_ProductStock(ProductStockChangeVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var ProductVO = ProductProvider.Get(Model.ProductId);
                    var StockBefore = ProductVO.Stock ?? 0;

                    ProductVO.Stock = Model.NewStockValue;

                    ProductProvider.Edit(ProductVO, AdminId);

                    ProductStockActivityProvider.Add(new PRODUCT_STOCK_ACTIVITY()
                    {
                        ProductId = Model.ProductId,
                        StockBefore = StockBefore,
                        StockAfter = Model.NewStockValue,
                        Remarks = Model.Remarks,
                    }, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("Edit", "Product", new { Id = Model.ProductId });
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
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Edit_AddImage(HttpPostedFileBase[] file, FormCollection form)
        {
            long ProductId = long.Parse(form["Id"]);

            try
            {
                var AdminId = (long)Session["ID"];

                //Image Uploading
                if (file != null && file.Count() > 0)
                {
                    List<string> filepaths = new List<string>();
                    foreach (var f in file)
                    {
                        if (f != null)
                        {
                            var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");

                            var OriginalImage = Image.FromStream(f.InputStream, true, true);
                            var FilePath = FileHelper.SaveImage(OriginalImage, f.FileName, postfix);

                            var size = Math.Min(OriginalImage.Width, OriginalImage.Height);
                            var ThumbnailImage = ImageHelper.CropImage(OriginalImage, size, size, true);
                            ThumbnailImage = ImageHelper.ResizeImage(ThumbnailImage, 300, 300);
                            FileHelper.SaveImage(ThumbnailImage, "thumb_" + f.FileName, postfix);

                            filepaths.Add(FilePath);
                        }
                    }

                    int DisplayOrder = 1;

                    var ProductImages = ProductImageProvider.GetAllByProductId(ProductId);
                    if (ProductImages.Count() > 0)
                    {
                        DisplayOrder = (ProductImages.Last().DisplayOrder ?? 0) + 1;
                    }

                    foreach (var filepath in filepaths)
                    {
                        ProductImageProvider.Add(new PRODUCT_IMAGE()
                        {
                            ProductId = ProductId,
                            URL = filepath,
                            DisplayOrder = DisplayOrder
                        }, AdminId);

                        DisplayOrder++;
                    }
                }

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("Edit", "Product", new { Id = ProductId });
        }

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Edit_DeleteImage(long ProductId, long ProductImageId)
        {
            var AdminId = (long)Session["ID"];

            ProductImageProvider.Delete(ProductImageId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("Edit", "Product", new { Id = ProductId });
        }

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult Delete(long Id)
        {
            var AdminId = (long)Session["ID"];

            var ProductVO = ProductProvider.Get(Id);
            ProductVO.Status = STATUS_CODE.INACTIVE;

            ProductProvider.Edit(ProductVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Product");
        }

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult ArrangeImage(long Id)
        {
            var v = ProductProvider.Get(Id);

            ProductVM model = new ProductVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Description = v.Description,
                Name = v.Name,
                Price = (int)v.Price,
                ProductCode = v.ProductCode,
                Size = v.Size,
                Stock = v.Stock,
                Weight = v.Weight,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult ArrangeImage_EditOrder(long ProductImageId, int DisplayOrder)
        {
            var AdminId = (long)Session["ID"];

            var ProductImageVO = ProductImageProvider.Get(ProductImageId);
            ProductImageVO.DisplayOrder = DisplayOrder;

            ProductImageProvider.Edit(ProductImageVO, AdminId);

            return Json("Successfully done.", JsonRequestBehavior.DenyGet);
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult ProductImageList_Read([CustomDataSourceRequest]DataSourceRequest request, long ProductId)
        {
            var List = ProductImageProvider.GetAllByProductId(ProductId);

            var result = List.ToDataSourceResult(request, m => new ProductImageVM { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult _Status(long Id)
        {
            var model = new ProductStatusVM();
            model.ProductId = Id;

            var OrderProducts = OrderProductProvider.GetAllByProductId(Id);

            var LastReset = ProductOrderStatusResetProvider.GetLatest(Id);
            if (LastReset != null)
            {
                OrderProducts = OrderProducts.Where(m => m.CreatedAt > LastReset.ResetDate);
                model.LastReset_DT = LastReset.ResetDate.Value.ToString("dd MMM yyyy HH:mm");
            }

            foreach (System.Reflection.FieldInfo field in typeof(ORDER_STATUS_CODE).GetFields())
            {
                var OrderStatus = field.GetValue(null).ToString();
                var qty = OrderProducts.Where(m => m.ORDER.Status == OrderStatus).Sum(m => m.Quantity);

                model.OrderStatusList.Add(OrderStatus, qty ?? 0);
            }

            return View(model);
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult _OrderListByProductAndStatus(long ProductId, string Status = "")
        {
            Status = Status == "" ? ORDER_STATUS_CODE.PENDING_PAYMENT : Status;

            var OrderProducts = OrderProductProvider.GetAllByProductId(ProductId).Where(m => m.ORDER.Status == Status);

            var LastReset = ProductOrderStatusResetProvider.GetLatest(ProductId);
            if (LastReset != null)
            {
                OrderProducts = OrderProducts.Where(m => m.CreatedAt > LastReset.ResetDate);
            }

            var model = OrderProducts.Select(m => new OrderProductVM()
            {
                Name = m.Name,
                Quantity = m.Quantity ?? 0,
                OrderId = m.OrderId,
                OrderCode = m.ORDER.OrderCode
            }).ToList();

            return View(model);
        }

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public JsonResult ResetOrderStatus(long ProductId)
        {
            var AdminId = (long)Session["ID"];

            try
            {
                ProductOrderStatusResetProvider.Add(new PRODUCT_ORDER_STATUS_RESET()
                {
                    ProductId = ProductId,
                    ResetDate = DateTime.Now
                }, AdminId);

                return Json("Successfully done.");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult SetNew(long Id)
        {
            var v = ProductProvider.Get(Id);

            ProductNewVM model = new ProductNewVM()
            {
                ProductId = v.Id,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(7)
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public ActionResult SetNew(ProductNewVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(Model.ValidFrom > Model.ValidTo)
                    {
                        throw new Exception("Valid From is greater than Valid To");
                    }

                    var AdminId = (long)Session["ID"];
                    
                    ProductNewProvider.Delete_ByProductId(Model.ProductId);

                    ProductNewProvider.Add(new PRODUCT_TRENDING()
                    {
                        ProductId = Model.ProductId,
                        ValidFrom = Model.ValidFrom,
                        ValidTo = Model.ValidTo
                    }, AdminId);

                    TempData["Message"] = "Successfully done.";
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
        [UserAuthorize(AccessModule.Product, "EDIT")]
        public JsonResult UnsetNew(long ProductId)
        {
            try
            {
                ProductNewProvider.Delete_ByProductId(ProductId);

                return Json("Successfully done.");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult _StockActivity(long ProductId)
        {
            ViewBag.ProductId = ProductId;

            return View();
        }

        [UserAuthorize(AccessModule.Product, "VIEW")]
        public ActionResult ProductStockActivityList_Read([CustomDataSourceRequest]DataSourceRequest request, long ProductId)
        {
            var List = ProductStockActivityProvider.GetAll().Where(m => m.ProductId == ProductId);

            DataSourceResult result = List.ToDataSourceResult(request, v => new ProductStockActivityVM
            {
                CreatedAt = v.CreatedAt,
                StockBefore = v.StockBefore,
                StockAfter = v.StockAfter,
                Remarks = v.Remarks
            });

            return Json(result);
        }

    }
}