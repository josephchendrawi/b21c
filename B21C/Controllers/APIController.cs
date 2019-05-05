using B21C.EF;
using B21C.Lib.Provider;
using B21C.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Controllers
{
    public class APIController : Controller
    {
        ProductProvider ProductProvider = new ProductProvider();
        ProductImageProvider ProductImageProvider = new ProductImageProvider();

        [HttpGet]
        public ActionResult Products(int Page = 1, int Size = 12)
        {
            var result = new List<ProductVM>();

            var products = ProductProvider.GetAll_DTO().OrderByDescending(m => m.ProductCode)
                                .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in products)
            {
                result.Add(new ProductVM
                {
                    Id = v.Id,
                    LastUpdAt = v.LastUpdAt,
                    Description = v.Description,
                    Name = v.Name,
                    Price = (int)v.Price,
                    Sales = v.Sales ?? false,
                    SalesDiscount = (int)(v.SalesDiscount ?? 0),
                    TotalPrice = (int)((v.Price ?? 0) - (v.Sales == true ? (v.SalesDiscount ?? 0) : 0)),
                    ProductCode = v.ProductCode,
                    Size = v.Size,
                    Stock = v.Stock,
                    Weight = v.Weight,
                    MainProductImage = (ProductImageProvider.GetMainImageByProductId(v.Id) ?? new PRODUCT_IMAGE()).URL
                });
            }
            
            return Json(new APIResponse()
            {
                Result = result,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Product(long Id)
        {
            var v = ProductProvider.Get(Id);

            var result = new ProductDetailsPageVM()
            {
                Id = v.Id,
                Description = v.Description,
                Name = v.Name,
                Price = (int)v.Price,
                Sales = v.Sales ?? false,
                SalesDiscount = (int)(v.SalesDiscount ?? 0),
                TotalPrice = (int)((v.Price ?? 0) - (v.Sales == true ? (v.SalesDiscount ?? 0) : 0)),
                ProductCode = v.ProductCode,
                Size = v.Size,
                Stock = v.Stock,
                Weight = v.Weight,
                Point = v.Point ?? 0,

                ProductImageList = ProductImageProvider.GetAllByProductId(v.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList(),
                
            };

            return Json(new APIResponse()
            {
                Result = result,
            }, JsonRequestBehavior.AllowGet);
        }
    }

    public class APIResponse
    {
        public object Result { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}