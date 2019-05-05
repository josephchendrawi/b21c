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
using B21C.Helper.AccessControl;
using B21C.Helper.Constants;
using B21C.Helper;
using B21C.Lib.Common.DTO;

namespace B21C.Controllers
{
    public class OrderController : BaseAdminController
    {
        AdminProvider AdminProvider = new AdminProvider();
        OrderProvider OrderProvider = new OrderProvider();
        ProductProvider ProductProvider = new ProductProvider();
        OrderProductProvider OrderProductProvider = new OrderProductProvider();
        ProductStockLogProvider ProductStockLogProvider = new ProductStockLogProvider();
        UserProvider UserProvider = new UserProvider();

        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult List(string TableName = "All")
        {
            ViewBag.TableName = TableName;
            return View();
        }

        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult OrderList_Read([CustomDataSourceRequest]DataSourceRequest request, string TableName = "All")
        {
            if (request.PageSize > 2500) { request.PageSize = 2500; }
            request.RenameRequestFilterSortMember("CreatedBy", "CreatedBy_Name");

            IQueryable<ORDER_DTO> List;
            
            if (TableName == "B21C")
            {
                List = OrderProvider.GetAll_DTO("b21c", "Admin");
            }
            else if (TableName == "B21C Store")
            {
                List = OrderProvider.GetAll_DTO("b21c", "Store");
            }
            else if (TableName == "Bagance")
            {
                List = OrderProvider.GetAll_DTO("bagance");
            }
            else
            {
                List = OrderProvider.GetAll_DTO();
            }

            DataSourceResult result = List.ToDataSourceResult(request, v => new OrderVM
            {
                Id = v.Id,
                BookedBy = v.BookedBy,
                CreatedAt = v.CreatedAt,
                LastUpdAt = v.LastUpdAt,
                Address = v.Address,
                ContactNo = v.ContactNo,
                ExpirationDate = v.ExpirationDate,
                FlgAdminLogo = v.FlgAdminLogo,
                OrderCode = v.OrderCode,
                Receiver = v.Receiver,
                Sender = v.Sender,
                ShippingFee = v.ShippingFee,
                TotalPrice = v.TotalPrice,
                GrandTotal = v.GrandTotal,
                TotalWeight = v.TotalWeight,
                TrackingNo = v.TrackingNo,
                Status = v.Status,
                PaymentMethod = v.PaymentMethod,
                PaymentExpired = v.PaymentExpired,
                Shipping = v.Shipping,
                Discount = v.Discount,
                AdditionalDiscount = v.AdditionalDiscount,
                PrepareShipmentDate = v.PrepareShipmentDate,
                BankAccountName = v.BankAccountName,
                UserPaymentAmount = v.UserPaymentAmount ?? 0,
                CreatedBy = v.CreatedBy_Name,
                PackedCount = v.PackedCount ?? 0,
                PackingCode = v.PackingCode
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult CreateOrUpdate(long Id = 0)
        {
            CreateOrUpdateOrderVM model = new CreateOrUpdateOrderVM();

            if (Id != 0)
            {
                var v = OrderProvider.Get(Id);

                model.Id = v.Id;
                model.BookedBy = v.BookedBy;
                model.Shipping = v.Shipping;
                model.ShippingFee = (int)(v.ShippingFee ?? 0);
                model.Discount = (int)(v.Discount ?? 0);
                model.PaymentMethod = v.PaymentMethod;
                model.AdditionalDiscount = (int)(v.AdditionalDiscount ?? 0);
                
                //get Existing Order Product
                foreach (var op in OrderProductProvider.GetAllByOrderId(Id))
                {
                    model.ProductList.Add(new OrderProductVM()
                    {
                        OrderProductId = op.Id,
                        Price = (int)op.Price,
                        Quantity = op.Quantity ?? 0,
                        Name = op.Name,
                        Weight = op.Weight ?? 0
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult CreateOrUpdate(CreateOrUpdateOrderVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];
                    
                    var OrderVO = new ORDER();
                    if (Model.Id == 0)
                    {
                        OrderVO.TotalPrice = 0;
                        OrderVO.TotalWeight = 0;
                        OrderVO.ExpirationDate = DateTime.Now.AddDays(2);
                        OrderVO.Status = ORDER_STATUS_CODE.PENDING_PAYMENT;
                    }
                    else
                    {
                        OrderVO = OrderProvider.Get(Model.Id);
                    }

                    OrderVO.BookedBy = Model.BookedBy;
                    OrderVO.Shipping = Model.Shipping;
                    OrderVO.ShippingFee = Model.ShippingFee;

                    if (AccessControl.IsAccessable((long)Session["Id"], AccessModule.Order_EditDiscount))
                    {
                        OrderVO.Discount = Model.Discount;
                        OrderVO.AdditionalDiscount = Model.AdditionalDiscount;
                    }

                    OrderVO.PaymentMethod = Model.PaymentMethod;

                    if (Model.Id == 0)
                    {
                        Model.Id = OrderProvider.Add(OrderVO, AdminId);

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("AddProduct", "Order", new { Id = Model.Id });
                    }
                    else
                    {
                        OrderProvider.Edit(OrderVO, AdminId);

                        Recalculate_Order_TotalPriceAndWeight(OrderVO.Id);

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("CreateOrUpdate", "Order", new { Id = Model.Id });
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

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult AddProduct(long Id)
        {
            var model = new UpdateOrderProductVM();

            var v = OrderProvider.Get(Id);

            model.Id = v.Id;
            model.BookedBy = v.BookedBy;
            model.Shipping = v.Shipping;
            model.ShippingFee = v.ShippingFee ?? 0;
            model.Discount = v.Discount ?? 0;
            model.PaymentMethod = v.PaymentMethod;
            model.AdditionalDiscount = v.AdditionalDiscount ?? 0;

            //get Existing Order Product
            foreach (var op in OrderProductProvider.GetAllByOrderId(Id))
            {
                model.ProductList.Add(new OrderProductVM()
                {
                    OrderProductId = op.Id,
                    Price = (int)op.Price,
                    Quantity = op.Quantity ?? 0,
                    Name = op.Name,
                    Weight = op.Weight ?? 0,
                    ProductId = op.ProductId
                });
            }

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult AddProduct(UpdateOrderProductVM Model, FormCollection form)
        {
            //populate OrderProductList by Model.ProductIds
            List<ORDER_PRODUCT> OrderProductList = new List<ORDER_PRODUCT>();
            try
            {
                if (!string.IsNullOrEmpty(Model.AddedProductS))
                {
                    foreach (var v in Model.AddedProductS.Split('|'))
                    {
                        if (v != "")
                        {
                            OrderProductList.Add(new ORDER_PRODUCT()
                            {
                                ProductId = long.Parse(v.Split('#')[0]),
                                Quantity = int.Parse(v.Split('#')[1])
                            });
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Something is wrong with Order Product list.");
            }

            try
            {
                //check if Selected Product's stock availability
                if (ProductProvider.CheckStockAvailability(OrderProductList) == false)
                {
                    throw new Exception("Insufficient stock for selected product.");
                }

                var AdminId = (long)Session["ID"];                    

                //get ordered product details
                foreach (var v in OrderProductList)
                {
                    var Product = ProductProvider.Get(v.ProductId);
                    v.Price = (int)((Product.Price ?? 0) - (Product.Sales == true ? (Product.SalesDiscount ?? 0) : 0));
                    v.Name = Product.Name;
                    v.Weight = Product.Weight;
                    v.Point = Product.Point;
                    v.Color = Product.Color;
                    v.Modal = Product.ModalPrice;
                    v.ProductCode = Product.ProductCode;

                    if (AccessControl.IsAccessable((long)Session["Id"], AccessModule.Order_ProductDisc35000))
                        v.Price = (Product.Sales == true ? v.Price : v.Price - 35000);
                }

                //add Order Product details
                var ExistingOrderProducts = OrderProductProvider.GetAllByOrderId(Model.Id);
                foreach (var v in OrderProductList)
                {
                    var ExistingOrderProduct = ExistingOrderProducts.Where(m => m.ProductId == v.ProductId).FirstOrDefault();
                    if (ExistingOrderProduct != null)
                    {
                        ExistingOrderProduct = OrderProductProvider.Get(ExistingOrderProduct.Id);
                        ExistingOrderProduct.Quantity += v.Quantity;

                        OrderProductProvider.Edit(ExistingOrderProduct, AdminId);
                    }
                    else
                    {
                        v.OrderId = Model.Id;

                        OrderProductProvider.Add(v, AdminId);
                    }
                }

                //recalculate TotalPrice & TotalWeight
                Recalculate_Order_TotalPriceAndWeight(Model.Id);

                //cut Product's stock value
                foreach (var v in OrderProductList)
                {
                    var ProductVO = ProductProvider.Get(v.ProductId);
                    ProductVO.Stock = ProductVO.Stock - v.Quantity;

                    ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.BOOK);
                }

                TempData["Message"] = "Successfully done.";
                return RedirectToAction("CreateOrUpdate", "Order", new { Id = Model.Id });
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            foreach (var op in OrderProductProvider.GetAllByOrderId(Model.Id))
            {
                Model.ProductList.Add(new OrderProductVM()
                {
                    OrderProductId = op.Id,
                    Price = (int)op.Price,
                    Quantity = op.Quantity ?? 0,
                    Name = op.Name,
                    Weight = op.Weight ?? 0,
                    ProductId = op.ProductId
                });
            }

            return View(Model);
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult _UpdateProduct(long OrderProductId)
        {
            var model = new OrderProductVM();

            var op = OrderProductProvider.Get(OrderProductId);

            model.OrderProductId = op.Id;
            model.Name = op.Name;
            model.Price = (int)(op.Price ?? 0);
            model.Weight = op.Weight ?? 0;
            model.Quantity = op.Quantity ?? 0;
            model.ProductId = op.ProductId;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult _UpdateProduct(OrderProductVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var OrderProductVO = OrderProductProvider.Get(Model.OrderProductId);

                    //ChangeAmount = gap between new and old
                    var QuantityChangeAmount = Model.Quantity - OrderProductVO.Quantity;

                    OrderProductVO.Quantity = Model.Quantity;

                    //add quantity
                    if (QuantityChangeAmount > 0)
                    {
                        //check Product's stock availability
                        var OrderProductList = new List<ORDER_PRODUCT>();
                        OrderProductList.Add(new ORDER_PRODUCT() { ProductId = OrderProductVO.ProductId, Quantity = QuantityChangeAmount });
                        if (ProductProvider.CheckStockAvailability(OrderProductList) == false)
                        {
                            throw new Exception("Insufficient stock for selected product.");
                        }

                        OrderProductProvider.Edit(OrderProductVO, AdminId);

                        //cut Product's stock value
                        var ProductVO = ProductProvider.Get(OrderProductVO.ProductId);
                        ProductVO.Stock = ProductVO.Stock - QuantityChangeAmount;

                        ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.BOOK);
                    }
                    //cut quantity
                    else if (QuantityChangeAmount < 0)
                    {
                        OrderProductProvider.Edit(OrderProductVO, AdminId);

                        //add back Product's stock value
                        var ProductVO = ProductProvider.Get(OrderProductVO.ProductId);
                        ProductVO.Stock = ProductVO.Stock + (QuantityChangeAmount * -1);

                        ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.CANCEL);
                    }

                    //recalculate TotalPrice & TotalWeight
                    Recalculate_Order_TotalPriceAndWeight(OrderProductVO.OrderId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("_UpdateProduct", "Order", new { OrderProductId = Model.OrderProductId });
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

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult RemoveProduct(long OrderProductId)
        {
            var AdminId = (long)Session["ID"];

            var OrderProductVO = OrderProductProvider.Get(OrderProductId);

            OrderProductProvider.Delete(OrderProductId);

            //recalculate TotalPrice & TotalWeight
            Recalculate_Order_TotalPriceAndWeight(OrderProductVO.OrderId);
            
            //add back Product's stock value
            var ProductVO = ProductProvider.Get(OrderProductVO.ProductId);
            ProductVO.Stock = ProductVO.Stock + OrderProductVO.Quantity;
            ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.CANCEL);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("CreateOrUpdate", "Order", new { Id = OrderProductVO.OrderId });
        }

        [NonAction]
        private void Recalculate_Order_AdditionalDiscount(long OrderId)
        {
            var AdminId = (long)Session["ID"];
            var OrderVO = OrderProvider.Get(OrderId);
            
            var OrderProducts = OrderProductProvider.GetAll().Where(m => m.OrderId == OrderId);
            
            OrderVO.AdditionalDiscount = OrderProducts.Sum(m => m.Quantity * 
                                             (m.PRODUCT.Sales == true ? Constant.AdditionalDiscount_PerDiscountedProduct : Constant.AdditionalDiscount_PerProduct)
                                         );

            OrderProvider.Edit(OrderVO, AdminId);
        }

        [NonAction]
        private void Recalculate_Order_TotalPriceAndWeight(long OrderId)
        {
            if (!AccessControl.IsAccessable((long)Session["Id"], AccessModule.Order_EditDiscount))
                Recalculate_Order_AdditionalDiscount(OrderId);
            
            var AdminId = (long)Session["ID"];

            var OrderProducts = OrderProductProvider.GetAllByOrderId(OrderId);

            var OrderVO = OrderProvider.Get(OrderId);
            OrderVO.TotalPrice = OrderProducts.Sum(m => ((m.Price ?? 0) - (OrderVO.Discount ?? 0)) * m.Quantity);
            OrderVO.TotalWeight = OrderProducts.Sum(m => (m.Weight ?? 0) * m.Quantity);

            OrderProvider.Edit(OrderVO, AdminId);
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult Shipment(long Id)
        {
            var v = OrderProvider.Get(Id);

            if (v.Status != ORDER_STATUS_CODE.PENDING_PAYMENT && v.Status != ORDER_STATUS_CODE.PREPARE_SHIPMENT)
            {
                return View("Admin_Unauthorized");
            }

            ShipOrderVM model = new ShipOrderVM()
            {
                Id = v.Id,
                Address = v.Address,
                ContactNo = v.ContactNo,
                FlgAdminLogo = v.FlgAdminLogo ?? false,
                Sender = v.Sender,
                Receiver = v.Receiver,
                ShippingCode = v.ShippingCode == null ? "" : v.ShippingCode.Replace(v.Shipping.ToUpper() + "-", ""),
                ShippingFee = v.ShippingFee ?? 0,
                Note = v.Note,
                PackingCode = v.PackingCode,
                OrderInfo = new OrderInfoVM()
                {
                    BookedBy = v.BookedBy,
                    OrderCode = v.OrderCode,
                    PaymentMethod = v.PaymentMethod,
                    Receiver = v.Receiver,
                    Sender = v.Sender,
                    Shipping = v.Shipping,
                    ShippingFee = v.ShippingFee ?? 0,
                    TotalPrice = (v.TotalPrice ?? 0) + (v.PaymentCode ?? 0),
                    GrandTotal = (v.TotalPrice ?? 0) + (v.ShippingFee ?? 0) - (v.AdditionalDiscount ?? 0) + (v.PaymentCode ?? 0),
                    TotalWeight = v.TotalWeight ?? 0,
                    ShippingCode = v.ShippingCode,
                    AdditionalDiscount = v.AdditionalDiscount ?? 0,
                    BankAccountName = v.BankAccountName,
                    UserPaymentAmount = v.UserPaymentAmount ?? 0
                }
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult Shipment(ShipOrderVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var OrderVO = OrderProvider.Get(Model.Id);
                    OrderVO.Address = Model.Address;
                    OrderVO.Sender = Model.Sender;
                    OrderVO.ContactNo = Model.ContactNo;
                    OrderVO.Receiver = Model.Receiver;
                    OrderVO.FlgAdminLogo = Model.FlgAdminLogo;
                    OrderVO.ShippingCode = Model.OrderInfo.Shipping.ToUpper() + "-" + Model.ShippingCode;
                    OrderVO.ShippingFee = Model.ShippingFee;
                    OrderVO.Note = Model.Note;
                    OrderVO.PackingCode = Model.PackingCode;

                    OrderVO.PaymentMethod = Model.OrderInfo.PaymentMethod;
                    OrderVO.Shipping = Model.OrderInfo.Shipping;

                    if (OrderVO.Status == ORDER_STATUS_CODE.PENDING_PAYMENT)
                    {
                        OrderVO.PrepareShipmentDate = DateTime.Now;
                    }

                    OrderVO.Status = ORDER_STATUS_CODE.PREPARE_SHIPMENT;

                    OrderProvider.Edit(OrderVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Order");
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

        [UserAuthorize(AccessModule.Order_Shipped, "VIEW")]
        public ActionResult Shipped(long Id)
        {
            var v = OrderProvider.Get(Id);

            if (v.Status != ORDER_STATUS_CODE.PREPARE_SHIPMENT && v.Status != ORDER_STATUS_CODE.SHIPPED)
            {
                return View("Admin_Unauthorized");
            }

            ShippedOrderVM model = new ShippedOrderVM()
            {
                Id = v.Id,
                TrackingNo = v.TrackingNo,
                ShippingDate = v.ShippingDate ?? DateTime.Now,
                OrderInfo = new OrderInfoVM()
                {
                    BookedBy = v.BookedBy,
                    OrderCode = v.OrderCode,
                    PaymentMethod = v.PaymentMethod,
                    Receiver = v.Receiver,
                    Sender = v.Sender,
                    Shipping = v.Shipping,
                    ShippingFee = v.ShippingFee ?? 0,
                    TotalPrice = (v.TotalPrice ?? 0) + (v.PaymentCode ?? 0),
                    GrandTotal = (v.TotalPrice ?? 0) + (v.ShippingFee ?? 0) - (v.AdditionalDiscount ?? 0) + (v.PaymentCode ?? 0),
                    TotalWeight = v.TotalWeight ?? 0,
                    ShippingCode = v.ShippingCode,
                    AdditionalDiscount = v.AdditionalDiscount ?? 0,
                    BankAccountName = v.BankAccountName,
                    UserPaymentAmount = v.UserPaymentAmount ?? 0
                }
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Order_Shipped, "EDIT")]
        public ActionResult Shipped(ShippedOrderVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var OrderVO = OrderProvider.Get(Model.Id);
                    OrderVO.TrackingNo = Model.TrackingNo;
                    OrderVO.ShippingDate = Model.ShippingDate;

                    OrderVO.Status = ORDER_STATUS_CODE.SHIPPED;

                    OrderProvider.Edit(OrderVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "Order");
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

        [UserAuthorize(AccessModule.Order, "DELETE")]
        public ActionResult Cancel(long Id)
        {
            var AdminId = (long)Session["ID"];

            var OrderVO = OrderProvider.Get(Id);

            if (OrderVO.Status != ORDER_STATUS_CODE.PENDING_PAYMENT && OrderVO.Status != ORDER_STATUS_CODE.PREPARE_SHIPMENT)
            {
                return View("Admin_Unauthorized");
            }

            OrderVO.Status = ORDER_STATUS_CODE.CANCELED;

            OrderProvider.Edit(OrderVO, AdminId);

            //add back Product's stock value
            var Order_ProductList = OrderProductProvider.GetAllByOrderId(Id);
            foreach (var v in Order_ProductList.GroupBy(m => m.ProductId).Select(m => new { Id = m.Key, Count = m.Sum(n => n.Quantity) }))
            {
                var ProductVO = ProductProvider.Get(v.Id);

                ProductVO.Stock = ProductVO.Stock + v.Count;

                ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.CANCEL);
            }

            //add back the balance user used
            var UserBalanceTrx = new UserBalanceTrxProvider().GetAll("b21c").Where(m => m.Remarks.Contains("Use as Payment for Order : " + OrderVO.OrderCode)).FirstOrDefault();
            if(UserBalanceTrx != null)
            {                
                UserProvider.AddBalance(UserBalanceTrx.UserId ?? 0, (UserBalanceTrx.Amount ?? 0) * -1, "Cancel Order : " + OrderVO.OrderCode, AdminId);
            }

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Order");
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult Extend(long Id)
        {
            var AdminId = (long)Session["ID"];

            var OrderVO = OrderProvider.Get(Id);

            if (OrderVO.Status != ORDER_STATUS_CODE.PENDING_PAYMENT)
            {
                return View("Admin_Unauthorized");
            }

            OrderVO.ExpirationDate = DateTime.Now.AddDays(1);

            OrderProvider.Edit(OrderVO, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Order");
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult Delivered(long Id)
        {
            var AdminId = (long)Session["ID"];

            var OrderVO = OrderProvider.Get(Id);

            if (OrderVO.Status != ORDER_STATUS_CODE.SHIPPED)
            {
                return View("Admin_Unauthorized");
            }

            OrderVO.Status = ORDER_STATUS_CODE.DELIVERED;

            OrderProvider.Edit(OrderVO, AdminId);
            
            //add RESELLER Point (one time)
            var Buyer = UserProvider.Get(OrderVO.CreatedBy.Value);
            if (Buyer != null && Buyer.Type == USER_TYPE.RESELLER)
            {
                var AddPointAmount = 0;
                foreach (var v in OrderProductProvider.GetAllByOrderId(OrderVO.Id))
                {
                    AddPointAmount += ((v.Quantity ?? 0) * (v.Point ?? 0));
                }
                UserProvider.AddPoint(OrderVO.CreatedBy.Value, AddPointAmount, AdminId);
            }

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Order");
        }

        [UserAuthorize(AccessModule.Order, "EDIT")]
        public ActionResult Returned(long Id)
        {
            var AdminId = (long)Session["ID"];

            var OrderVO = OrderProvider.Get(Id);

            if (OrderVO.Status != ORDER_STATUS_CODE.SHIPPED)
            {
                return View("Admin_Unauthorized");
            }

            OrderVO.Status = ORDER_STATUS_CODE.RETURNED;

            OrderProvider.Edit(OrderVO, AdminId);

            //add back Product's stock value
            //var Order_ProductList = OrderProductProvider.GetAllByOrderId(Id);
            //foreach (var v in Order_ProductList.GroupBy(m => m.ProductId).Select(m => new { Id = m.Key, Count = m.Count() }))
            //{
            //    var ProductVO = ProductProvider.Get(v.Id);
            //    ProductVO.Stock = ProductVO.Stock + v.Count;

            //    ProductProvider.Edit(ProductVO, AdminId);
            //}

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "Order");
        }

        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult View(long Id)
        {
            var v = OrderProvider.Get(Id);

            OrderVM model = new OrderVM()
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                CreatedAt = v.CreatedAt,
                BookedBy = v.BookedBy,
                Address = v.Address,
                ContactNo = v.ContactNo,
                ExpirationDate = v.ExpirationDate,
                FlgAdminLogo = v.FlgAdminLogo,
                OrderCode = v.OrderCode,
                Receiver = v.Receiver,
                Sender = v.Sender,
                ShippingFee = v.ShippingFee,
                TotalPrice = v.TotalPrice + (v.PaymentCode ?? 0),
                GrandTotal = v.TotalPrice + v.ShippingFee - v.AdditionalDiscount + (v.PaymentCode ?? 0),
                TotalWeight = v.TotalWeight,
                TrackingNo = v.TrackingNo,
                Status = v.Status,
                PaymentMethod = v.PaymentMethod,
                ProductList = new List<OrderProductVM>(),
                Shipping = v.Shipping,
                ShippingCode = v.ShippingCode,
                Discount = v.Discount,
                PrepareShipmentDate = v.PrepareShipmentDate,
                Note = v.Note,
                AdditionalDiscount = v.AdditionalDiscount,
                BankAccountName = v.BankAccountName,
                UserPaymentAmount = v.UserPaymentAmount ?? 0,
                PackingCode = v.PackingCode
            };

            foreach (var op in OrderProductProvider.GetAllByOrderId(Id))
            {
                model.ProductList.Add(new OrderProductVM()
                {
                    OrderProductId = op.Id,
                    Price = (int)op.Price,
                    Quantity = op.Quantity ?? 0,
                    Name = op.Name,
                    Weight = op.Weight ?? 0,
                    ProductId = op.ProductId,
                    ProductCode = ProductProvider.Get(op.ProductId).ProductCode
                });
            }

            return View(model);
        }

        [UserAuthorize(AccessModule.Order_CheckPackingCount, "EDIT")]
        public ActionResult IncreasePackingCount(string Ids)
        {
            var AdminId = (long)Session["ID"];

            var IdList = Ids.Split(',').Select(long.Parse).ToList();
            foreach (var v in IdList)
            {
                var OrderVO = OrderProvider.Get(v);

                OrderVO.PackedCount = OrderVO.PackedCount == null ? 1 : OrderVO.PackedCount + 1;
                OrderProvider.Edit(OrderVO, AdminId);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult DecreasePackingCount(string Ids)
        {
            var AdminId = (long)Session["ID"];

            var IdList = Ids.Split(',').Select(long.Parse).ToList();
            foreach (var v in IdList)
            {
                var OrderVO = OrderProvider.Get(v);

                OrderVO.PackedCount = OrderVO.PackedCount == null ? 0 : OrderVO.PackedCount - 1;
                OrderProvider.Edit(OrderVO, AdminId);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [UserAuthorize(AccessModule.MutationCheck, "VIEW")]
        public ActionResult MutationCheck()
        {
            var model = new MutationCheckVM();
            model.BCAMutation = new List<BankMutationItem>();
            model.MandiriMutation = new List<BankMutationItem>();
            model.BRIMutation = new List<BankMutationItem>();
            model.MutationCheckOrderList = new List<MutationCheck_OrderVM>();
            model.PrepareShipmentDate = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.MutationCheck, "VIEW")]
        public ActionResult MutationCheck(MutationCheckVM model, HttpPostedFileBase bca, HttpPostedFileBase mandiri, HttpPostedFileBase bri)
        {
            model.BCAMutation = new List<BankMutationItem>();
            model.MandiriMutation = new List<BankMutationItem>();
            model.BRIMutation = new List<BankMutationItem>();

            var BankMutations = new List<BankMutationItem>();

            if (bca != null)
            {
                var bca_csv_lines = new List<string>();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(bca.InputStream))
                {
                    while (!reader.EndOfStream)
                    {
                        bca_csv_lines.Add(reader.ReadLine());
                    }
                }

                if (bca_csv_lines.Count() > 0)
                {
                    model.BCAMutation = BankMutation.GetBCA(bca_csv_lines).Where(m => m.Credit != null && m.Credit > 0).ToList();

                    BankMutations = BankMutations.Union(model.BCAMutation.Where(m => m.Credit != null && m.Credit > 0).ToList()).ToList();
                }

            }
            if (mandiri != null)
            {
                var mandiri_csv_lines = new List<string>();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(mandiri.InputStream))
                {
                    while (!reader.EndOfStream)
                    {
                        mandiri_csv_lines.Add(reader.ReadLine());
                    }
                }

                if (mandiri_csv_lines.Count() > 0)
                {
                    model.MandiriMutation = BankMutation.GetMandiri(mandiri_csv_lines).Where(m => m.Credit != null && m.Credit > 0).ToList();
                    
                    BankMutations = BankMutations.Union(model.MandiriMutation).ToList();
                }

            }
            if (bri != null)
            {
                string bri_html = "";

                using (System.IO.StreamReader reader = new System.IO.StreamReader(bri.InputStream))
                {
                    while (!reader.EndOfStream)
                    {
                        bri_html += reader.ReadLine();
                    }
                }

                if (!string.IsNullOrEmpty(bri_html))
                {
                    model.BRIMutation = BankMutation.GetBRI(bri_html).Where(m => m.Credit != null && m.Credit > 0).ToList();

                    BankMutations = BankMutations.Union(model.BRIMutation).ToList();
                }

            }
            
            var OrderList = OrderProvider.GetAll_DTO().Where(m=>m.PrepareShipmentDate!= null
                            && m.PrepareShipmentDate.Value.Year == model.PrepareShipmentDate.Year
                            && m.PrepareShipmentDate.Value.Month == model.PrepareShipmentDate.Month
                            && m.PrepareShipmentDate.Value.Day == model.PrepareShipmentDate.Day);

            var ByWebsite_UserIds = AdminProvider.GetAllUserByWebsite("b21c").Where(m => (m.ROLE != null && m.ROLE.Name.Contains("b21c")) || m.Type == USER_TYPE.RESELLER).Select(m => m.Id);
            OrderList = OrderList.Where(m => m.CreatedBy != null && ByWebsite_UserIds.Contains(m.CreatedBy.Value) && m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT);

            var CheckedOrderList = from v in OrderList.ToList()
                                   join e in BankMutations on v.UserPaymentAmount equals e.Credit into tmp_e
                                   from f in tmp_e.DefaultIfEmpty()
                                   join g in BankMutations on v.GrandTotal equals g.Credit into tmp_g
                                   from h in tmp_g.DefaultIfEmpty()
                                   select new MutationCheck_OrderVM
                                   {
                                       Id = v.Id,
                                       BookedBy = v.BookedBy,
                                       CreatedAt = v.CreatedAt,
                                       LastUpdAt = v.LastUpdAt,
                                       Address = v.Address,
                                       ContactNo = v.ContactNo,
                                       OrderCode = v.OrderCode,
                                       Receiver = v.Receiver,
                                       Sender = v.Sender,
                                       ShippingFee = v.ShippingFee,
                                       TotalPrice = v.TotalPrice,
                                       GrandTotal = v.GrandTotal,
                                       TotalWeight = v.TotalWeight,
                                       TrackingNo = v.TrackingNo,
                                       Status = v.Status,
                                       PaymentMethod = v.PaymentMethod,
                                       PaymentExpired = v.PaymentExpired,
                                       Shipping = v.Shipping,
                                       Discount = v.Discount,
                                       AdditionalDiscount = v.AdditionalDiscount,
                                       PrepareShipmentDate = v.PrepareShipmentDate,
                                       BankAccountName = v.BankAccountName,
                                       UserPaymentAmount = v.UserPaymentAmount ?? 0,
                                       CreatedBy = v.CreatedBy_Name,
                                       MatchedBank = f != null ? f.Bank : (h != null ? h.Bank : null),
                                   };

            model.MutationCheckOrderList = CheckedOrderList.ToList();

            return View(model);
        }


        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult MassivePrinting(string Ids)
        {
            var Model = new List<PrintOrderVM>();

            var IdList = Ids.Split(',').Select(long.Parse).ToList();
            var List = OrderProvider.GetAll_DTO().Where(m => IdList.Contains(m.Id) && m.FlgAdminLogo != null).ToList();

            foreach (var v in List)
            {
                var ProductList = new List<OrderProductVM>();
                foreach (var op in OrderProductProvider.GetAllByOrderId(v.Id))
                {
                    ProductList.Add(new OrderProductVM()
                    {
                        OrderProductId = op.Id,
                        Price = (int)op.Price,
                        Quantity = op.Quantity ?? 0,
                        Name = op.Name,
                        Weight = op.Weight ?? 0,
                        ProductId = op.ProductId,
                        ProductCode = ProductProvider.Get(op.ProductId).ProductCode
                    });
                }

                Model.Add(new PrintOrderVM()
                {
                    OrderId = v.Id,
                    Address = v.Address,
                    ContactNo = v.ContactNo,
                    OrderCode = v.OrderCode,
                    Receiver = v.Receiver,
                    Sender = v.Sender,
                    ProductList = ProductList,
                    ShippingCode = v.ShippingCode,
                    Note = v.Note,
                    PackingCode = v.PackingCode,
                });
            }

            return View(Model);
        }


        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult MassiveOrderCancelling(string Ids)
        {
            var AdminId = (long)Session["ID"];

            var Model = new List<PrintOrderVM>();

            var IdList = Ids.Split(',').Select(long.Parse).ToList();
            foreach (var Id in IdList)
            {
                var OrderVO = OrderProvider.Get(Id);
                if (OrderVO != null && OrderVO.Status == ORDER_STATUS_CODE.PENDING_PAYMENT)
                {
                    OrderVO.Status = ORDER_STATUS_CODE.CANCELED;

                    OrderProvider.Edit(OrderVO, AdminId);

                    //add back Product's stock value
                    var Order_ProductList = OrderProductProvider.GetAllByOrderId(OrderVO.Id);
                    foreach (var v in Order_ProductList.GroupBy(m => m.ProductId).Select(m => new { Id = m.Key, Count = m.Sum(n => n.Quantity) }))
                    {
                        var ProductVO = ProductProvider.Get(v.Id);

                        ProductVO.Stock = ProductVO.Stock + v.Count;

                        ProductProvider.Edit(ProductVO, AdminId, PRODUCT_MODIFICATION_ACTION.CANCEL);
                    }

                    //add back the balance user used
                    var UserBalanceTrx = new UserBalanceTrxProvider().GetAll("b21c").Where(m => m.Remarks.Contains("Use as Payment for Order : " + OrderVO.OrderCode)).FirstOrDefault();
                    if (UserBalanceTrx != null)
                    {
                        UserProvider.AddBalance(UserBalanceTrx.UserId ?? 0, (UserBalanceTrx.Amount ?? 0) * -1, "Cancel Order : " + OrderVO.OrderCode, AdminId);
                    }
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}