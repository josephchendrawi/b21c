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
    public class UserBalanceTopUpController : BaseAdminController
    {
        UserBalanceTopUpProvider UserBalanceTopUpProvider = new UserBalanceTopUpProvider();
        UserProvider UserProvider = new UserProvider();

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize(AccessModule.Buyer, "VIEW")]
        public ActionResult UserBalanceTopUpList_Read([CustomDataSourceRequest]DataSourceRequest request)
        {
            request.RenameRequestFilterSortMember("Username", "USER.Username");
            request.RenameRequestFilterSortMember("UserName", "USER.Name");

            var List = UserBalanceTopUpProvider.GetAll("b21c");

            DataSourceResult result = List.ToDataSourceResult(request, v => new UserBalanceTopUpVM
            {
                Id = v.Id,
                LastUpdAt = v.LastUpdAt,
                Status = v.Status,
                AccountName = v.AccountName,
                Amount = v.Amount,
                Bank = v.Bank,
                TransferDateTime = v.TransferDateTime,
                UserId = v.UserId,
                Username = v.USER.Username,
                UserName = v.USER.Name,
            });

            return Json(result);
        }

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Approve(long Id)
        {
            var AdminId = (long)Session["ID"];

            UserBalanceTopUpProvider.ChangeStatus(Id, TOP_UP_STATUS.APPROVED, AdminId);

            var UserBalanceTopUp = UserBalanceTopUpProvider.Get(Id);
            UserProvider.AddBalance(UserBalanceTopUp.UserId.Value, (UserBalanceTopUp.Amount ?? 0), "Top Up Approved for BalanceTopUp Id : " + UserBalanceTopUp.Id, AdminId);
            
            //send email
            //

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "UserBalanceTopUp");
        }

        [UserAuthorize(AccessModule.Buyer, "EDIT")]
        public ActionResult Reject(long Id)
        {
            var AdminId = (long)Session["ID"];

            UserBalanceTopUpProvider.ChangeStatus(Id, TOP_UP_STATUS.REJECTED, AdminId);

            //send email
            //

            TempData["Message"] = "Successfully done.";
            return RedirectToAction("List", "UserBalanceTopUp");
        }

    }
}