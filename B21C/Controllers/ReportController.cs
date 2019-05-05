using B21C.EF;
using B21C.Enums;
using B21C.Helper;
using B21C.Lib.Common.Constants;
using B21C.Lib.Provider;
using B21C.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Controllers
{
    public class ReportController : BaseAdminController
    {
        AdminProvider AdminProvider = new AdminProvider();
        OrderProvider OrderProvider = new OrderProvider();
        ProductProvider ProductProvider = new ProductProvider();
        OrderProductProvider OrderProductProvider = new OrderProductProvider();
        LookupProvider LookupProvider = new LookupProvider();
        UserProvider UserProvider = new UserProvider();

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult OrderProductReport()
        {
            var Products = ProductProvider.GetAll();

            var model = new OverallProductReportVM();

            model.TotalCurrentStock = Products.Sum(m => (int)m.Stock);
            model.TotalCurrentStockPrice = Products.Sum(m => (m.ModalPrice ?? 0) * (decimal)m.Stock);

            return View(model);
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult OrderProductReportList_Read([CustomDataSourceRequest]DataSourceRequest request, int? inMonth, int? inYear, string ByRole = "")
        {
            inMonth = inMonth ?? DateTime.Now.Month;
            inYear = inYear ?? DateTime.Now.Year;
           
            var List = OrderProvider.GetAll_DTO().Where(m => m.ShippingDate.Value.Month == inMonth && m.ShippingDate.Value.Year == inYear && (m.Status == ORDER_STATUS_CODE.SHIPPED || (m.Status == ORDER_STATUS_CODE.DELIVERED)));

            if(ByRole == "Admin")
            {
                List = List.Where(m => m.CreatedBy_Role.ToLower().Contains("admin") || (m.CreatedBy_Type == USER_TYPE.RESELLER && m.CreatedBy_Website.ToLower() == "b21c"));
            }
            else if (ByRole == "Partner")
            {
                List = List.Where(m => !m.CreatedBy_Role.ToLower().Contains("admin") && !(m.CreatedBy_Type == USER_TYPE.RESELLER && m.CreatedBy_Website.ToLower() == "b21c"));
            }

            var ListVM = List.ToList().Select(v => new OrderProductReportItem()
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
                TotalWeight = v.TotalWeight,
                TrackingNo = v.TrackingNo,
                Status = v.Status,
                PaymentMethod = v.PaymentMethod,
                Shipping = v.Shipping,
                AdditionalDiscount = v.AdditionalDiscount * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                PrepareShipmentDate = v.PrepareShipmentDate,
                Note = v.Note,
                ShippingCode = v.ShippingCode,
                DiscountForEachProduct = v.Discount,
                ProductList = v.ORDER_PRODUCT.Select(p => new OrderProductVM_forOrderProductReport { ProductCode = p.ProductCode ?? p.PRODUCT.ProductCode, Quantity = p.Quantity ?? 0, ModalPrice = (int)(p.Modal ?? (p.PRODUCT.ModalPrice ?? 0)), Name = p.Name ?? p.PRODUCT.Name, }).ToList(),
                TotalModalPrice = v.ORDER_PRODUCT.Sum(m => m.Modal ?? (m.PRODUCT.ModalPrice ?? 0)),
                ProductCount = v.ORDER_PRODUCT.Sum(m => m.Quantity ?? 0),
                CreatedBy = v.CreatedBy_Name,
                TotalPartnerDiscount = v.TotalPartnerDiscount ?? 0,
                //ProductCodeList = string.Join(", ", v.ORDER_PRODUCT.Select(m => m.ProductCode + " x" + m.Quantity)),
                //TotalDiscountForEachProduct = v.ORDER_PRODUCT.Sum(m => m.Quantity) * v.Discount,
                ShippingDate = v.ShippingDate
            });

            DataSourceResult result = ListVM.ToDataSourceResult(request);

            return Json(result);
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult OrderPackingShift(string Date, int? Shift)
        {
            var Model = new PackingShiftReportVM();

            if (!String.IsNullOrWhiteSpace(Date) && Shift != null)
            {
                Model.Date = DateTime.ParseExact(Date, "dd-MM-yyyy", null);
                Model.Shift = (PackingShift)Shift;
            }
            else
            {
                Model.Date = DateTime.Now;
                if (DateTime.Now.Hour < 14)
                {
                    Model.Shift = PackingShift.Afternoon;
                }
                else
                {
                    Model.Shift = PackingShift.Evening;
                }
            }

            return View(Model);
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult OrderPackingShiftList_Read([CustomDataSourceRequest]DataSourceRequest request, string Date, int Shift)
        {
            var ShiftTime = EnumExtensions.GetEnumDescription((PackingShift)Shift).Split('-');
            TimeSpan ShiftStart = new TimeSpan(int.Parse(ShiftTime[0].Split(':')[0]), int.Parse(ShiftTime[0].Split(':')[1]), 00);
            TimeSpan ShiftEnd = new TimeSpan(int.Parse(ShiftTime[1].Split(':')[0]), int.Parse(ShiftTime[1].Split(':')[1]), 59);

            var Date_ = DateTime.ParseExact(Date, "dd-MM-yyyy", null);

            var StartDateTime = new DateTime(Date_.Year, Date_.Month, Date_.Day, ShiftStart.Hours, ShiftStart.Minutes, ShiftStart.Seconds);
            if (ShiftStart > ShiftEnd)
                StartDateTime = StartDateTime.AddDays(-1);
            var EndDateTime = new DateTime(Date_.Year, Date_.Month, Date_.Day, ShiftEnd.Hours, ShiftEnd.Minutes, ShiftEnd.Seconds);

            var List = OrderProductProvider.GetAll().Where(m => m.ORDER.PrepareShipmentDate >= StartDateTime && m.ORDER.PrepareShipmentDate < EndDateTime && m.ORDER.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT)
                        .Select(v => new OrderProductVM_forPackingShiftReport()
                        {
                            PrepareShipmentDate = v.ORDER.PrepareShipmentDate,
                            Name = v.Name,
                            Address = v.ORDER.Address,
                            OrderCode = v.ORDER.OrderCode,
                            OrderProductId = v.Id,
                            Price = v.Price ?? 0,
                            ProductCode = v.PRODUCT.ProductCode,
                            ProductId = v.ProductId,
                            Quantity = v.Quantity ?? 0,
                            Receiver = v.ORDER.Receiver,
                            Sender = v.ORDER.Sender,
                            Weight = v.Weight ?? 0
                        });
            
            DataSourceResult result = List.ToDataSourceResult(request);

            return Json(result);
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult ShippingReport()
        {
            var model = new ShippingReportVM();

            var LookupVO = LookupProvider.Get("ShippingReport_Note");
            if(LookupVO != null)
                model.Note = LookupVO.Value;

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult ShippingReport(ShippingReportVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var LookupVO = LookupProvider.Get("ShippingReport_Note");

                    if (LookupVO == null)
                    {
                        LookupVO = new LOOKUP();
                        LookupVO.Value = Model.Note;

                        LookupVO.Type = "Note";
                        LookupVO.Name = "ShippingReport_Note";
                        LookupVO.ActiveFlag = true;

                        LookupProvider.Add(LookupVO, AdminId);
                    }
                    else
                    {
                        LookupVO.Value = Model.Note;

                        LookupProvider.Edit(LookupVO, AdminId);
                    }

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("ShippingReport", "Report");
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

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult ShippingReportList_Read([CustomDataSourceRequest]DataSourceRequest request, int? inMonth, int? inYear)
        {
            inMonth = inMonth ?? DateTime.Now.Month;
            inYear = inYear ?? DateTime.Now.Year;

            var List = OrderProvider.GetAll_DTO().Where(m => m.ShippingDate != null && (m.ShippingDate.Value.Month == inMonth && m.ShippingDate.Value.Year == inYear) && (m.Status == ORDER_STATUS_CODE.SHIPPED || (m.Status == ORDER_STATUS_CODE.DELIVERED)));

            var ListVM = List.ToList().Select(v => new ShippingReportItem()
            {
                OrderId = v.Id,
                OrderCode = v.OrderCode,
                Sender = v.Sender,
                Receiver = v.Receiver,
                ShippingDate = v.ShippingDate.Value,
                ShippingFee = (int)(v.ShippingFee ?? 0),
                ShippingMethod = v.Shipping,
                TrackingNo = v.TrackingNo ?? "",
                TrackingNoCount = string.IsNullOrEmpty(v.TrackingNo) ? 0 : v.TrackingNo.Count(f => f == '\n') + 1,
            });

            DataSourceResult result = ListVM.ToDataSourceResult(request);

            return Json(result);
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult MonthlyReport(int Month = 0, int Year = 0, string ByRole = "")
        {
            if (Month == 0 || Year == 0)
            {
                Month = DateTime.Now.Month;
                Year = DateTime.Now.Year;
            }

            ViewBag.inMonth = Month;
            ViewBag.inYear = Year;
            ViewBag.ByRole = ByRole;

            return View();
        }

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult OrderReportList_Read([CustomDataSourceRequest]DataSourceRequest request, int? inMonth, int? inYear, string ByRole = "")
        {
            inMonth = inMonth ?? DateTime.Now.Month;
            inYear = inYear ?? DateTime.Now.Year;

            var List = OrderProvider.GetAll_DTO().Where(m => m.ShippingDate.Value.Month == inMonth && m.ShippingDate.Value.Year == inYear && (m.Status == ORDER_STATUS_CODE.SHIPPED || (m.Status == ORDER_STATUS_CODE.DELIVERED || m.Status == ORDER_STATUS_CODE.RETURNED)));
            
            if (ByRole == "Admin")
            {
                List = List.Where(m => m.CreatedBy_Role.ToLower().Contains("admin") || (m.CreatedBy_Type == USER_TYPE.RESELLER && m.CreatedBy_Website.ToLower() == "b21c"));
            }
            else if (ByRole == "Partner")
            {
                List = List.Where(m => !m.CreatedBy_Role.ToLower().Contains("admin"));
            }

            var ListVM = List.Select(v => new OrderVM()
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
                ShippingFee = v.ShippingFee * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalPrice = v.TotalPrice * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalPriceWithAdditionalDiscount = v.TotalPriceWithAdditionalDiscount * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                GrandTotal = v.GrandTotal * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalWeight = v.TotalWeight,
                TrackingNo = v.TrackingNo,
                Status = v.Status,
                PaymentMethod = v.PaymentMethod,
                PaymentExpired = v.PaymentExpired,
                Shipping = v.Shipping,
                Discount = v.Discount * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                PrepareShipmentDate = v.PrepareShipmentDate,
                TotalPartnerDiscount = v.TotalPartnerDiscount ?? 0,
                ShippingDate = v.ShippingDate
            });

            DataSourceResult result = ListVM.ToDataSourceResult(request);

            return Json(result);
        }


        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult PartnerOrderReport()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Order, "VIEW")]
        public ActionResult Partner_OrderReportList_Read([CustomDataSourceRequest]DataSourceRequest request, string ByWebsite)
        {
            if (request.PageSize > 2500) { request.PageSize = 2500; }

            var List = OrderProvider.GetAll_DTO();

            if (ByWebsite != "")
            {
                var ByWebsite_UserIds = AdminProvider.GetAllUserByWebsite(ByWebsite).Select(m => m.Id);
                List = List.Where(m => m.CreatedBy != null && ByWebsite_UserIds.Contains(m.CreatedBy.Value));
            }

            List = List.Where(m => (m.Status == ORDER_STATUS_CODE.SHIPPED || m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT) || (m.Status == ORDER_STATUS_CODE.DELIVERED || m.Status == ORDER_STATUS_CODE.RETURNED));

            DataSourceResult result = List.ToDataSourceResult(request, v => new Partner_OrderReportVM
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
                ShippingFee = v.ShippingFee * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalPrice = v.TotalPrice * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalPriceWithAdditionalDiscount = v.TotalPriceWithAdditionalDiscount * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                GrandTotal = v.GrandTotal * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                TotalWeight = v.TotalWeight,
                TrackingNo = v.TrackingNo,
                Status = v.Status,
                PaymentMethod = v.PaymentMethod,
                PaymentExpired = v.PaymentExpired,
                Shipping = v.Shipping,
                Discount = v.Discount * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
                AdditionalDiscount = v.AdditionalDiscount,
                PrepareShipmentDate = v.PrepareShipmentDate,
                BankAccountName = v.BankAccountName,
                UserPaymentAmount = v.UserPaymentAmount ?? 0,
                TotalPartnerDiscount = (v.TotalPartnerDiscount ?? 0) * (v.Status == ORDER_STATUS_CODE.RETURNED ? -1 : 1),
            });

            return Json(result);
        }

    }
}