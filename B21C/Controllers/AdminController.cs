using B21C.EF;
using B21C.Enums;
using B21C.Lib.Common.Constants;
using B21C.Lib.Provider;
using B21C.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Owin.Security;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace B21C.Controllers
{
    public class AdminController : BaseAdminController
    {
        AdminProvider AdminProvider = new AdminProvider();
        OrderProvider OrderProvider = new OrderProvider();
        RoleProvider RoleProvider = new RoleProvider();
        UserProvider UserProvider = new UserProvider();
        UserBalanceTopUpProvider UserBalanceTopUpProvider = new UserBalanceTopUpProvider();

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [HttpPost]
        public ActionResult KendoExcelSave(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBuyer_NewOrder()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var List = OrderProvider.GetAll();

                var ByWebsite_BuyerIds = UserProvider.GetAllBuyerByWebsite("b21c").Select(m => m.Id);
                var SuperAdminId = AdminProvider.GetAllAdminByWebsite("b21c").Select(m => m.Id).First();
                List = List.Where(m => m.CreatedBy != null && (ByWebsite_BuyerIds.Contains(m.CreatedBy.Value) || (m.BookedBy == "Guest Buyer" && m.CreatedBy == SuperAdminId)));

                List = List.Where(m => m.Status == ORDER_STATUS_CODE.PENDING_PAYMENT);// || (m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT && (m.UserPaymentAmount == 0 && m.PaymentMethod == "Balance")));
                var TotalCount = List.Count();

                List = List.OrderByDescending(m => m.Id);

                var Result = new NotificationAjaxResult()
                {
                    TotalCount = TotalCount,
                    NotificationList = List.ToList().Select(m => new { Id = m.Id, OrderCode = m.OrderCode, BookedBy = m.BookedBy, ProductCount = m.ORDER_PRODUCT.Sum(n => n.Quantity) }).Cast<object>().ToList()
                };

                result.Success = true;
                result.Result = Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetTopUpPending()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var List = UserBalanceTopUpProvider.GetAll("b21c");

                List = List.Where(m => m.Status == TOP_UP_STATUS.PENDING);
                var TotalCount = List.Count();

                List = List.OrderByDescending(m => m.Id);

                var Result = new NotificationAjaxResult()
                {
                    TotalCount = TotalCount,
                    NotificationList = List.ToList().Select(m => new { Amount = (m.Amount ?? 0).ToString("n0"), Username = m.USER.Username, CreatedAt = m.CreatedAt.Value.ToString("dd/MMM HH:mm"), Bank = m.Bank }).Cast<object>().ToList()
                };

                result.Success = true;
                result.Result = Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetBuyer_FullyPaidOrder()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var List = OrderProvider.GetAll();
                
                var ByWebsite_BuyerIds = UserProvider.GetAllBuyerByWebsite("b21c").Select(m => m.Id);
                var SuperAdminId = AdminProvider.GetAllAdminByWebsite("b21c").Select(m => m.Id).First();
                List = List.Where(m => m.CreatedBy != null && (ByWebsite_BuyerIds.Contains(m.CreatedBy.Value) || (m.BookedBy == "Guest Buyer" && m.CreatedBy == SuperAdminId)));

                List = List.Where(m => m.Status == ORDER_STATUS_CODE.PENDING_PAYMENT && m.ReceivedPaymentConfirmationAt != null);// || (m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT && (m.UserPaymentAmount == 0 && m.PaymentMethod == "Balance")));
                var TotalCount = List.Count();
                //var ByWebsite_BuyerIds = UserProvider.GetAllBuyerByWebsite("b21c").Select(m => m.Id);
                //var SuperAdminId = AdminProvider.GetAllAdminByWebsite("b21c").Select(m => m.Id).First();
                //List = List.Where(m => m.CreatedBy != null && (ByWebsite_BuyerIds.Contains(m.CreatedBy.Value) || (m.BookedBy == "Guest Buyer" && m.CreatedBy == SuperAdminId)));

                //List = List.Where(m => (m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT && (m.UserPaymentAmount == 0 && m.PaymentMethod == "Balance")));
                //var TotalCount = List.Count();

                List = List.OrderByDescending(m => m.Id);

                var Result = new NotificationAjaxResult()
                {
                    TotalCount = TotalCount,
                    NotificationList = List.ToList().Select(m => new { Id = m.Id, OrderCode = m.OrderCode, BookedBy = m.BookedBy, ProductCount = m.ORDER_PRODUCT.Sum(n => n.Quantity) }).Cast<object>().ToList()
                };

                result.Success = true;
                result.Result = Result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        public ActionResult Login(string ReturnURL = "")
        {
            LoginModel model = new LoginModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, FormCollection form, string ReturnURL = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Admin = AdminProvider.Login(model.Username, model.Password, "b21c");

                    if (Admin != null)
                    {
                        //Session["Username"] = Admin.Username;
                        //Session["ID"] = Admin.Id;
                        //Session["UserType"] = Admin.Type;

                        var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.NameIdentifier, Admin.Id.ToString()),
                            new Claim(ClaimTypes.Name, Admin.Username),
                            new Claim(ClaimTypes.Role, Admin.Type),
                        }, "ApplicationCookie");

                        AuthenticationManager.SignIn(new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = false, //isPersistent,
                            ExpiresUtc = DateTime.UtcNow.AddDays(7)
                        }, identity);

                        if (ReturnURL == null || ReturnURL == "")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return Redirect(ReturnURL);
                        }
                    }
                    else
                        TempData["Message"] = "Invalid Username or Password.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(model);
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            AuthenticationManager.SignOut("ApplicationCookie", "ExternalCookie");
            return RedirectToAction("Login", "Admin");
        }

        [UserAuthorize(AccessModule.Admin, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Admin, "VIEW")]
        public ActionResult AdminList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            request.RenameRequestFilterSortMember("RoleName", "ROLE.Name");

            var List = AdminProvider.GetAllAdminByWebsite("b21c");

            DataSourceResult result = List.ToDataSourceResult(request, v => new AdminVM
            {
                Id = v.Id,
                Name = v.Name,
                Username = v.Username,
                RoleName = v.ROLE != null ? v.ROLE.Name : "-",
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Admin, "ADD")]
        public ActionResult Add()
        {
            AdminAddVM model = new AdminAddVM();
            
            var RoleList = new List<SelectListItem>();
            foreach (var itm in RoleProvider.GetAll())
            {
                RoleList.Add(new SelectListItem() { Value = itm.Id.ToString(), Text = itm.Name });
            }
            ViewBag.RoleList = RoleList;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Admin, "ADD")]
        public ActionResult Add(AdminAddVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var AdminVO = new USER();
                    AdminVO.Name = Model.Name;
                    AdminVO.Username = Model.Username;
                    AdminVO.RoleId = Model.RoleId;
                    AdminVO.Password = Model.Password;

                    AdminVO.Status = STATUS_CODE.ACTIVE;
                    AdminVO.Website = "b21c";

                    if (AdminProvider.Get(AdminVO.Username, "b21c") != null)
                    {
                        throw new Exception("The Email is already exists.");
                    }

                    var result = AdminProvider.Add(AdminVO, AdminId);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("Edit", "Admin", new { Id = result });
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

            var RoleList = new List<SelectListItem>();
            foreach (var itm in RoleProvider.GetAll())
            {
                RoleList.Add(new SelectListItem() { Value = itm.Id.ToString(), Text = itm.Name });
            }
            ViewBag.RoleList = RoleList;

            return View(Model);
        }

        [UserAuthorize(AccessModule.Admin, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = AdminProvider.Get(Id);

            AdminEditVM model = new AdminEditVM()
            {
                Id = v.Id,
                Name = v.Name,
                Username = v.Username,
                RoleId = v.RoleId
            };

            var RoleList = new List<SelectListItem>();
            foreach (var itm in RoleProvider.GetAll())
            {
                RoleList.Add(new SelectListItem() { Value = itm.Id.ToString(), Text = itm.Name });
            }
            ViewBag.RoleList = RoleList;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Admin, "EDIT")]
        public ActionResult Edit(AdminEditVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var AdminVO = AdminProvider.Get(Model.Id);
                    AdminVO.Name = Model.Name;
                    AdminVO.Username = Model.Username;
                    AdminVO.RoleId = Model.RoleId;
                    if (Model.isPasswordChange)
                        AdminVO.Password = Model.Password;

                    AdminProvider.Edit(AdminVO, Model.isPasswordChange, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("Edit", "Admin", new { Id = Model.Id });
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

            var RoleList = new List<SelectListItem>();
            foreach (var itm in RoleProvider.GetAll())
            {
                RoleList.Add(new SelectListItem() { Value = itm.Id.ToString(), Text = itm.Name });
            }
            ViewBag.RoleList = RoleList;

            return View(Model);
        }

    }
}