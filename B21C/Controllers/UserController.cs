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
    public class UserController : BaseAdminController
    {
        UserProvider UserProvider = new UserProvider();
        UserBalanceTrxProvider UserBalanceTrxProvider = new UserBalanceTrxProvider();
        UserStatusProvider UserStatusProvider = new UserStatusProvider();

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult BuyerList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            request.RenameRequestFilterSortMember("UserStatus", "USER_STATUS.Name");

            var List = UserProvider.GetAllBuyerByWebsite("b21c");

            DataSourceResult result = List.ToDataSourceResult(request, v => new BuyerVM
            {
                Id = v.Id,
                Balance = (int)(v.Balance ?? 0),
                BirthDate = v.BirthDate,
                Username = v.Username,
                Gender = v.Gender,
                Name = v.Name,
                Phone = v.Phone,
                Point = v.Point ?? 0,
                Type = v.Type,
                UserStatus = v.USER_STATUS != null ? v.USER_STATUS.Name : "-",
                RegisteredAt = v.CreatedAt
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = UserProvider.Get(Id);

            BuyerEditVM model = new BuyerEditVM()
            {
                Id = v.Id,
                BirthDate = v.BirthDate,
                Username = v.Username,
                Gender = v.Gender,
                Name = v.Name,
                Phone = v.Phone,
                Point = v.Point ?? 0,
                UserStatusId = v.UserStatusId ?? 0,
            };

            var UserStatusList = new List<SelectListItem>();
            foreach (var us in UserStatusProvider.GetAll())
            {
                UserStatusList.Add(new SelectListItem() { Value = us.Id.ToString(), Text = us.Name });
            }
            ViewBag.UserStatusList = UserStatusList;

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Edit(BuyerEditVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    var UserVO = UserProvider.Get(Model.Id);
                    UserVO.BirthDate = Model.BirthDate;
                    UserVO.Username = Model.Username;
                    UserVO.Gender = Model.Gender;
                    UserVO.Name = Model.Name;
                    UserVO.Phone = Model.Phone;
                    UserVO.Point = Model.Point;

                    if (Model.isUserStatusChange)
                    {
                        UserVO.UserStatusId = Model.UserStatusId;
                        UserVO.USER_STATUS = null; //has to set to null because 'Get' Method got Include this USER_STATUS

                        //change status also change Point
                        var UserStatus = UserStatusProvider.Get(Model.UserStatusId);
                        UserVO.Point = UserStatus.PointNeeded;
                    }

                    UserProvider.Edit(UserVO, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "User");
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

            var UserStatusList = new List<SelectListItem>();
            foreach (var us in UserStatusProvider.GetAll())
            {
                UserStatusList.Add(new SelectListItem() { Value = us.Id.ToString(), Text = us.Name });
            }
            ViewBag.UserStatusList = UserStatusList;

            return View(Model);
        }

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult ChangeBalance(long Id)
        {
            var v = UserProvider.Get(Id);

            BuyerChangeBalanceVM model = new BuyerChangeBalanceVM()
            {
                Id = v.Id,
                Balance = (int)(v.Balance ?? 0),
                Username = v.Username,
                Name = v.Name,
            };

            return View(model);
        }

        [HttpPost]
        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult ChangeBalance(BuyerChangeBalanceVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = (long)Session["ID"];

                    UserProvider.UpdateBalance(Model.Id, Model.Balance, Model.Remarks, AdminId);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "User");
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

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult View(long Id)
        {
            var v = UserProvider.Get(Id);

            BuyerVM model = new BuyerVM()
            {
                Id = v.Id,
                Balance = (int)(v.Balance ?? 0),
                BirthDate = v.BirthDate,
                Username = v.Username,
                Gender = v.Gender,
                Name = v.Name,
                Phone = v.Phone,
                Point = v.Point ?? 0,
                Type = v.Type,
                UserStatus = v.USER_STATUS != null ? v.USER_STATUS.Name : "-",
            };

            return View(model);
        }

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult UserBalanceTrxList_Read([CustomDataSourceRequest]DataSourceRequest request, long UserId)
        {
            var List = UserBalanceTrxProvider.GetAll(UserId);

            DataSourceResult result = List.ToDataSourceResult(request, v => new UserBalanceTrxVM
            {
                Id = v.Id,
                Amount = v.Amount,
                LastUpdAt = v.LastUpdAt,
                Remarks = v.Remarks,
                UserId = v.UserId,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Buyer, "DELETE")]
        public ActionResult Inactive(long Id)
        {
            var AdminId = (long)Session["ID"];

            var v = UserProvider.Get(Id);
            v.Status = STATUS_CODE.INACTIVE;

            UserProvider.Edit(v, AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "User");
        }
        
        [UserAuthorize(AccessModule.Buyer, "DELETE")]
        public JsonResult InactiveMassive(string Ids)
        {
            var AdminId = (long)Session["ID"];
            
            var IdList = Ids.Split(',').Select(long.Parse).ToList();
            foreach (var Id in IdList)
            {
                var v = UserProvider.Get(Id);
                v.Status = STATUS_CODE.INACTIVE;

                UserProvider.Edit(v, AdminId);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult ResetPassword(long Id)
        {
            var AdminId = (long)Session["ID"];

            UserProvider.EditPassword(Id, "123456789", AdminId);

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "User");
        }

    }
}