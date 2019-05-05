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
    public class ProductStockLogController : BaseAdminController
    {
        ProductStockLogProvider ProductStockLogProvider = new ProductStockLogProvider();
        ProductProvider ProductProvider = new ProductProvider();

        [UserAuthorize(AccessModule.Report, "VIEW")]
        public ActionResult Index(string Date = "", string CompareDate = "")
        {
            var model = new ProductStockLogVM();

            if (!string.IsNullOrWhiteSpace(Date))
            {
                model.Date = DateTime.ParseExact(Date, "dd-MM-yyyy", null);
            }
            else
            {
                model.Date = DateTime.Now;
            }

            var LogList = ProductStockLogProvider.GetByDate(model.Date);

            if (LogList.Count() > 0)
            {
                var LatestLogOfThatDay = LogList.OrderByDescending(m => m.CreatedAt).First();

                //Canceled
                foreach (var v in LatestLogOfThatDay.Canceled.Split('|'))
                {
                    if (!string.IsNullOrWhiteSpace(v))
                        model.Canceled_Products.Add(v);
                }
                model.Canceled_Amount = model.Canceled_Products.Count();

                //Available
                foreach (var v in LatestLogOfThatDay.Available.Split('|').Except(model.Canceled_Products))
                {
                    if (!string.IsNullOrWhiteSpace(v))
                        model.Available_Products.Add(v);
                }
                model.Available_Amount = model.Available_Products.Count();

                //SoldOut
                foreach (var v in LatestLogOfThatDay.SoldOut.Split('|'))
                {
                    if (!string.IsNullOrWhiteSpace(v))
                        model.SoldOut_Products.Add(v);
                }
                model.SoldOut_Amount = model.SoldOut_Products.Count();

            }

            //comparation process
            if (!string.IsNullOrWhiteSpace(CompareDate))
            {
                model.CompareDate = DateTime.ParseExact(CompareDate, "dd-MM-yyyy", null);

                if (model.CompareDate > model.Date)
                {
                    TempData["Message"] = "Compare Date is greater than Date.";
                }
                else
                {
                    //loop until d-day
                    for (DateTime Compare_Date = model.CompareDate.Value; Compare_Date < model.Date; Compare_Date = Compare_Date.AddDays(1))
                    {
                        var Compare_LogList = ProductStockLogProvider.GetByDate(Compare_Date);

                        if (Compare_LogList.Count() > 0)
                        {
                            var Compare_LatestLogOfThatDay = Compare_LogList.OrderByDescending(m => m.CreatedAt).First();

                            var Compare_SoldOut_Products = new List<string>();
                            foreach (var v in Compare_LatestLogOfThatDay.SoldOut.Split('|'))
                            {
                                if (!string.IsNullOrWhiteSpace(v))
                                    Compare_SoldOut_Products.Add(v);
                            }

                            model.Addition_SoldOut_Products = model.Addition_SoldOut_Products.Union(model.SoldOut_Products.Except(Compare_SoldOut_Products)).Distinct().ToList();
                        }
                        //else
                        //{
                        //    TempData["Message"] = "No log was found at " + CompareDate + ".";
                        //}                
                    }
                }
            }

            return View(model);
        }

    }
}