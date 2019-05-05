using B21C.EF;
using B21C.Helper;
using B21C.Helper.Constants;
using B21C.Lib.BLL;
using B21C.Lib.Common.Constants;
using B21C.Lib.Provider;
using B21C.Models;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace B21C.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {
            this.ViewBag.ProductCategoryList = ProductCategoryProvider.GetAll().OrderBy(m => m.Name).Select(m => new ProductCategoryVM() { Id = m.Id, Name = m.Name });
        }

        ProductBLL ProductBLL = new ProductBLL();

        ProductImageProvider ProductImageProvider = new ProductImageProvider();
        BannerProvider BannerProvider = new BannerProvider();
        CatalogProvider CatalogProvider = new CatalogProvider();
        ProductNewProvider ProductNewProvider = new ProductNewProvider();
        UserProvider UserProvider = new UserProvider();
        ShoppingCartProvider ShoppingCartProvider = new ShoppingCartProvider();
        AddressBookProvider AddressBookProvider = new AddressBookProvider();
        UserBalanceTopUpProvider UserBalanceTopUpProvider = new UserBalanceTopUpProvider();
        OrderProvider OrderProvider = new OrderProvider();
        OrderProductProvider OrderProductProvider = new OrderProductProvider();
        ProductCategoryProvider ProductCategoryProvider = new ProductCategoryProvider();
        UserStatusProvider UserStatusProvider = new UserStatusProvider();
        LookupProvider LookupProvider = new LookupProvider();

        RajaOngkir RajaOngkir = new RajaOngkir();
        //ResiAWB ResiAWB = new ResiAWB();

        public ActionResult GetCityList()
        {
            return Json(RajaOngkir.GetCityList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubdistrictList(string city_id)
        {
            return Json(RajaOngkir.GetSubdistrictList(city_id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShippingFeeByCourier(string dest_subdistrict_id, string courier)
        {
            return Json(RajaOngkir.GetShippingFee(dest_subdistrict_id, courier), JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ShippingFee(string subdistrict_id)
        {
            var model = RajaOngkir.GetShippingFee(subdistrict_id);

            return View(model);
        }

        //public ActionResult _Tracking(string courier, string trackingno)
        //{
        //    ResiAWB_COURIER ResiAWB_COURIER = new ResiAWB_COURIER();

        //    if(courier.ToLower().Contains("jne"))
        //        ResiAWB_COURIER = ResiAWB_COURIER.jne;
        //    else if(courier.ToLower().Contains("pos"))
        //        ResiAWB_COURIER = ResiAWB_COURIER.pos_id;
        //    else if(courier.ToLower().Contains("tiki"))
        //        ResiAWB_COURIER = ResiAWB_COURIER.tiki;
        //    else
        //    {
        //        return Redirect("http://www.lacakresi.id/?noresi=" + trackingno);
        //    }

        //    var model = ResiAWB.GetTracking(ResiAWB_COURIER,trackingno);

        //    return View(model);
        //}

        public ActionResult _Tracking(string courier, string trackingno)
        {
            if (courier.ToLower().Contains("jne"))
                courier = "jne";
            else if (courier.ToLower().Contains("pos"))
                courier = "pos";
            else if (courier.ToLower().Contains("tiki"))
                courier = "tiki";
            else if (courier.ToLower().Contains("jnt"))
                courier = "jnt";

            var model = RajaOngkir.GetTracking(trackingno, courier.ToLower());

            return View(model);
        }

        [HttpPost]
        public ActionResult AddSubscriber(FormCollection form)
        {
            try
            {
                var Email = form["SubscriberEmail"];

                var NewsletterSubscriberProvider = new NewsletterSubscriberProvider();

                if (NewsletterSubscriberProvider.GetAll().Where(m => m.Email.ToLower() == Email.ToLower()).Count() == 0)
                {
                    var EmailSubscriberVO = new EMAIL_SUBSCRIBER();
                    EmailSubscriberVO.Email = Email;

                    EmailSubscriberVO.Status = STATUS_CODE.ACTIVE;

                    var result = NewsletterSubscriberProvider.Add(EmailSubscriberVO, 0);

                    if (result != 0)
                    {
                        TempData["Message"] = "You have successfully registered on our weekly catalog subscription.";
                    }
                }
                else
                {
                    TempData["Message"] = "Your email had already been registered.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GetTrackingNo(FormCollection form)
        {
            try
            {
                var OrderCode = form["OrderCode"];

                var Order = new OrderProvider().GetAll_DTO().Where(m => m.OrderCode == OrderCode).FirstOrDefault();
                if (Order != null)
                {
                    if (!string.IsNullOrWhiteSpace(Order.TrackingNo))
                    {
                        TempData["TrackingNo"] = Order.Shipping + " - " + Order.TrackingNo + " a.n " + Order.Receiver;
                    }
                    else
                    {
                        TempData["Message"] = "No. Resi masih belum tersedia.";
                    }
                }
                else
                {
                    TempData["Message"] = "Kode Order tidak ditemukan.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            var model = new HomepageVM();

            var BannerList = BannerProvider.GetAll().Where(m => m.Status == STATUS_CODE.PUBLISHED).OrderBy(m => m.Name);
            foreach (var v in BannerList)
            {
                model.BannerList.Add(new BannerVM
                {
                    Id = v.Id,
                    URL = v.URL,
                    Name = v.Name,
                    ImageURL = v.ImageURL
                });
            }

            var NewProductIdList = ProductNewProvider.GetAll_Valid_TrendingProductId().ToList();
            var NewProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => NewProductIdList.Contains(m.Id) && m.Stock > 0).OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode).Take(8);
            foreach (var v in NewProductList)
            {
                model.NewProductList.Add(new ProductVM
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

            var SalesProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => m.Sales == true && m.Stock > 0 && (m.Section == null || m.Section == "")).OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode).Take(4);
            foreach (var v in SalesProductList)
            {
                model.SalesProductList.Add(new ProductVM
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

            var SpecialPromoProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => m.Sales == true && m.Stock > 0 && (m.Section != null && m.Section.Contains(PRODUCT_SECTION.SPECIAL_PROMO))).OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode).Take(4);
            foreach (var v in SpecialPromoProductList)
            {
                model.SpecialPromoProductList.Add(new ProductVM
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

            var CatalogList = CatalogProvider.GetAll().OrderByDescending(m => m.CreatedAt).Take(2);
            foreach (var v in CatalogList)
            {
                model.CatalogList.Add(new CatalogVM
                {
                    Id = v.Id,
                    LastUpdAt = v.LastUpdAt,
                    Name = v.Name,
                    URL = v.URL,
                });
            }

            return View(model);
        }

        public ActionResult AllProducts(int Page = 1, int Size = 12, string Query = "", string OrderBy = "", string OrderDir = "desc", string WeightQuery = "")
        {
            var model = new ProductPageVM();
            model.Page = Page;
            model.Size = Size;
            model.Query = Query;
            model.OrderBy = OrderBy;
            model.OrderDir = OrderDir;

            var ProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);

            if (Query != "")
            {
                ProductList = ProductList.Where(m => m.Name.ToLower().Contains(Query.ToLower()));
            }
            if(WeightQuery != "")
            {
                try
                {
                    var WeightQ = int.Parse(WeightQuery);
                    model.WeightQuery = WeightQuery;
                    ProductList = ProductList.Where(m => m.Weight == WeightQ);
                }
                catch { }
            }

            if (OrderBy == "Price")
            {
                if (OrderDir == "asc")
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenBy(m => ((m.Price ?? 0) - (m.Sales == true ? (m.SalesDiscount ?? 0) : 0))).ThenByDescending(m => m.CreatedAt);
                else
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => ((m.Price ?? 0) - (m.Sales == true ? (m.SalesDiscount ?? 0) : 0))).ThenByDescending(m => m.CreatedAt);
            }
            else if (OrderBy == "Code")
            {
                if (OrderDir == "asc")
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenBy(m => m.ProductCode).ThenByDescending(m => m.CreatedAt);
                else
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode).ThenByDescending(m => m.CreatedAt);
            }
            else if (OrderBy == "Date")
            {
                if (OrderDir == "asc")
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenBy(m => m.CreatedAt);
                else
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.CreatedAt);
            }
            else
            {
                if (OrderDir == "asc")
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenBy(m => m.ProductCode).ThenByDescending(m => m.CreatedAt);
                else
                    ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode).ThenByDescending(m => m.CreatedAt);
            }

            var TotalCount = ProductList.Count();

            ProductList = ProductList.Skip((Page - 1) * Size).Take(Size);

            foreach (var v in ProductList)
            {
                model.ProductList.Add(new ProductVM
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

            var TotalPage = (int)Math.Ceiling((double)TotalCount / Size);

            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            if (Page <= 3)
            {
                for (int i = 1; i <= 5; i++)
                    model.AvailablePage.Add(i);
            }
            else if (Page > (TotalPage - 3))
            {
                for (int i = TotalPage - 5 + 1; i <= TotalPage; i++)
                    model.AvailablePage.Add(i);
            }
            else
            {
                for (int i = Page - 2; i <= Page + 2; i++)
                    model.AvailablePage.Add(i);
            }

            model.AvailablePage = model.AvailablePage.Where(m => m > 0 && m <= TotalPage).ToList();

            return View(model);
        }

        public ActionResult NewProducts(int Page = 1, int Size = 12)
        {
            var model = new ProductPageVM();
            model.Page = Page;
            model.Size = Size;

            var NewProductIdList = ProductNewProvider.GetAll_Valid_TrendingProductId().ToList();
            var ProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => NewProductIdList.Contains(m.Id));

            var TotalCount = ProductList.Count();

            ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode)
                                .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in ProductList)
            {
                model.ProductList.Add(new ProductVM
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

            var TotalPage = (int)Math.Ceiling((double)TotalCount / Size);

            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            if (Page <= 3)
            {
                for (int i = 1; i <= 5; i++)
                    model.AvailablePage.Add(i);
            }
            else if (Page > (TotalPage - 3))
            {
                for (int i = TotalPage - 5 + 1; i <= TotalPage; i++)
                    model.AvailablePage.Add(i);
            }
            else
            {
                for (int i = Page - 2; i <= Page + 2; i++)
                    model.AvailablePage.Add(i);
            }

            model.AvailablePage = model.AvailablePage.Where(m => m > 0 && m <= TotalPage).ToList();

            return View(model);
        }

        public ActionResult Sales(int Page = 1, int Size = 12)
        {
            var model = new ProductPageVM();
            model.Page = Page;
            model.Size = Size;

            var ProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => m.Sales == true && (m.Section == null || m.Section == ""));

            var TotalCount = ProductList.Count();

            ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode)
                                .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in ProductList)
            {
                model.ProductList.Add(new ProductVM
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

            var TotalPage = (int)Math.Ceiling((double)TotalCount / Size);

            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            if (Page <= 3)
            {
                for (int i = 1; i <= 5; i++)
                    model.AvailablePage.Add(i);
            }
            else if (Page > (TotalPage - 3))
            {
                for (int i = TotalPage - 5 + 1; i <= TotalPage; i++)
                    model.AvailablePage.Add(i);
            }
            else
            {
                for (int i = Page - 2; i <= Page + 2; i++)
                    model.AvailablePage.Add(i);
            }

            model.AvailablePage = model.AvailablePage.Where(m => m > 0 && m <= TotalPage).ToList();

            return View(model);
        }

        public ActionResult SpecialPromo(int Page = 1, int Size = 12)
        {
            var model = new ProductPageVM();
            model.Page = Page;
            model.Size = Size;

            var ProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => m.Sales == true && (m.Section != null && m.Section.Contains(PRODUCT_SECTION.SPECIAL_PROMO)));

            var TotalCount = ProductList.Count();

            ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode)
                                .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in ProductList)
            {
                model.ProductList.Add(new ProductVM
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

            var TotalPage = (int)Math.Ceiling((double)TotalCount / Size);

            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            if (Page <= 3)
            {
                for (int i = 1; i <= 5; i++)
                    model.AvailablePage.Add(i);
            }
            else if (Page > (TotalPage - 3))
            {
                for (int i = TotalPage - 5 + 1; i <= TotalPage; i++)
                    model.AvailablePage.Add(i);
            }
            else
            {
                for (int i = Page - 2; i <= Page + 2; i++)
                    model.AvailablePage.Add(i);
            }

            model.AvailablePage = model.AvailablePage.Where(m => m > 0 && m <= TotalPage).ToList();

            return View(model);
        }

        public ActionResult CategoryProducts(long Id, int Page = 1, int Size = 12)
        {
            var model = new CategoryProductPageVM();
            model.Page = Page;
            model.Size = Size;

            var ProductCategory = ProductCategoryProvider.Get(Id);
            model.CategoryId = ProductCategory.Id;
            model.CategoryName = ProductCategory.Name;

            var ProductList = ProductBLL.GetAll_DTO(Session["User_ID"] == null ? 0 : (long)Session["User_ID"]).Where(m => m.CategoryId == Id);

            var TotalCount = ProductList.Count();

            ProductList = ProductList.OrderBy(m => m.OutOfStock).ThenByDescending(m => m.ProductCode)
                                .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in ProductList)
            {
                model.ProductList.Add(new ProductVM
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

            var TotalPage = (int)Math.Ceiling((double)TotalCount / Size);

            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            if (Page <= 3)
            {
                for (int i = 1; i <= 5; i++)
                    model.AvailablePage.Add(i);
            }
            else if (Page > (TotalPage - 3))
            {
                for (int i = TotalPage - 5 + 1; i <= TotalPage; i++)
                    model.AvailablePage.Add(i);
            }
            else
            {
                for (int i = Page - 2; i <= Page + 2; i++)
                    model.AvailablePage.Add(i);
            }

            model.AvailablePage = model.AvailablePage.Where(m => m > 0 && m <= TotalPage).ToList();

            return View(model);
        }

        public ActionResult Product(int Id)
        {
            var v = ProductBLL.Get(Id, Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);

            ProductDetailsPageVM model = new ProductDetailsPageVM()
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

                MemberDiscount = ProductBLL.GetUserDiscount(Session["User_ID"] == null ? 0 : (long)Session["User_ID"])
            };

            return View(model);
        }

        public ActionResult Signup()
        {
            //if logged in, redirect to home
            if (Session["User_ID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            SignupVM model = new SignupVM();

            return View(model);
        }

        [HttpPost]
        public ActionResult Signup(SignupVM Model, FormCollection form)
        {
            //if logged in, redirect to home
            if (Session["User_ID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime? BirthDate = null;
                    if (!string.IsNullOrWhiteSpace(Model.BirthDate))
                    {
                        try
                        {
                            BirthDate = DateTime.ParseExact(Model.BirthDate, "dd/MM/yyyy", null);
                        }
                        catch
                        {
                            throw new Exception("The '" + Model.BirthDate + "' is not valid for Date of Birth");
                        }
                    }

                    var UserVO = new USER();
                    UserVO.BirthDate = BirthDate;
                    UserVO.Gender = Model.Gender;
                    UserVO.Name = Model.Name;
                    UserVO.Password = Model.Password;
                    UserVO.Phone = Model.Phone;
                    UserVO.Username = Model.Email;

                    var UserStatus_Initial = new UserStatusProvider().GetAll().Where(m => m.PointNeeded == 0).FirstOrDefault();
                    UserVO.UserStatusId = UserStatus_Initial == null ? 1 : UserStatus_Initial.Id; //initial
                    UserVO.Balance = 0;
                    UserVO.Point = 0;
                    //UserVO.Type = USER_TYPE.CUSTOMER;
                    UserVO.Type = USER_TYPE.RESELLER;
                    UserVO.Website = "b21c";

                    UserVO.Status = STATUS_CODE.ACTIVE;

                    if (UserProvider.Get(UserVO.Username, "b21c") != null)
                    {
                        throw new Exception("The Email is already taken.");
                    }
                    else
                    {
                        var result = UserProvider.Add(UserVO, 0);

                        if (result != 0)
                        {
                            //send email
                            string email_html = System.IO.File.ReadAllText(Server.MapPath(@"~/Content/email-html/signup-complete-email.html"));
                            email_html = email_html.Replace("#NAME#", Model.Name);
                            Helper.Email.SendGrid(Model.Email, "Bags21Century - Registrasi Berhasil", email_html);

                            Session["User_Username"] = UserVO.Username;
                            Session["User_ID"] = result;
                            Session["User_UserType"] = UserVO.Type;

                            if (Request.UrlReferrer.ToString().ToLower().Contains("/store/signup")
                                || (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString())))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                return Redirect(Request.UrlReferrer.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = string.Join(" \\n", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return View(Model);
        }

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            AjaxResult result = new AjaxResult();

            if (Session["User_ID"] == null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var User = UserProvider.Login(email, password, "b21c");

                        if (User != null)
                        {
                            //Session["User_Username"] = User.Username;
                            //Session["User_ID"] = User.Id;
                            //Session["User_UserType"] = User.Type;

                            var identity = new ClaimsIdentity(new[] {
                                new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()),
                                new Claim(ClaimTypes.Name, User.Username),
                                new Claim(ClaimTypes.Role, User.Type),
                            }, "ApplicationCookie");

                            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties()
                            {
                                AllowRefresh = true,
                                IsPersistent = false, //isPersistent,
                                ExpiresUtc = DateTime.UtcNow.AddDays(7)
                            }, identity);

                            result.Success = true;
                            result.Message = "You are logged in.";

                            if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                            {
                                result.ReturnURL = Url.Action("Index", "Home");
                            }
                            else
                            {
                                result.ReturnURL = Request.UrlReferrer.ToString();
                            }
                        }
                        else
                            result.Message = "Invalid Username or Password.";
                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.Message;
                    }
                }
            }

            return Json(result);
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie", "ExternalCookie");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyAccount()
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var UserId = (long)Session["User_ID"];

            var UserVO = new UserProvider().Get(UserId);

            var model = new MyAccountVM()
            {
                BirthDate = UserVO.BirthDate == null ? null : UserVO.BirthDate.Value.ToString("dd/MM/yyyy"),
                Email = UserVO.Username,
                Gender = UserVO.Gender,
                Name = UserVO.Name,
                Phone = UserVO.Phone,
                Balance = (int)(UserVO.Balance ?? 0),
                Point = UserVO.Point ?? 0,
                MemberStatus = UserVO.USER_STATUS.Name,
                MemberStatusPhotoURL = UserVO.USER_STATUS.IconImage,
            };

            foreach (var v in new AddressBookProvider().GetAll(UserId))
            {
                model.AddressBookList.Add(new AddressBookVM()
                {
                    Id = v.Id,
                    Address = v.Address,
                    AddressName = v.AddressName,
                    City = v.City,
                    Phone = v.Phone,
                    Postcode = v.Postcode,
                    Receiver = v.Receiver,
                    Subdistrict = v.Subdistrict,
                    SubdistrictId = v.SubdistrictId ?? 0,
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditAccount(UserVM Model, FormCollection form)
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = (long)Session["User_ID"];

                    var UserVO = new UserProvider().Get(UserId);

                    DateTime? BirthDate = null;
                    if (!string.IsNullOrWhiteSpace(Model.BirthDate))
                    {
                        try
                        {
                            BirthDate = DateTime.ParseExact(Model.BirthDate, "dd/MM/yyyy", null);
                        }
                        catch
                        {
                            throw new Exception("The '" + Model.BirthDate + "' is not valid for Date of Birth");
                        }
                    }

                    UserVO.BirthDate = BirthDate;
                    UserVO.Gender = Model.Gender;
                    UserVO.Name = Model.Name;
                    UserVO.Phone = Model.Phone;
                    UserVO.Username = Model.Email;

                    UserProvider.Edit(UserVO, UserId);

                    if (!string.IsNullOrWhiteSpace(Model.Password))
                    {
                        UserProvider.EditPassword(UserVO.Id, Model.Password, UserId);
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = string.Join(" \\n", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return RedirectToAction("MyAccount", "Home");
        }

        public ActionResult MyOrder(string Status = "", int Page = 1, int Size = 12)
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var UserId = (long)Session["User_ID"];

            var model = new MyOrderVM();
            model.Status = Status;

            var List = OrderProvider.GetAll_DTO();

            //filtering
            List = List.Where(m => m.CreatedBy != null && m.CreatedBy == UserId);
            if (Status == "PendingPayment")
            {
                List = List.Where(m => m.Status == ORDER_STATUS_CODE.PENDING_PAYMENT);
            }
            else if (Status == "PrepareShipment")
            {
                List = List.Where(m => m.Status == ORDER_STATUS_CODE.PREPARE_SHIPMENT);
            }
            else if (Status == "Shipped")
            {
                List = List.Where(m => m.Status == ORDER_STATUS_CODE.SHIPPED);
            }
            else if (Status == "Completed")
            {
                List = List.Where(m => m.Status == ORDER_STATUS_CODE.DELIVERED || m.Status == ORDER_STATUS_CODE.RETURNED);
            }
            else if (Status == "Canceled")
            {
                List = List.Where(m => m.Status == ORDER_STATUS_CODE.CANCELED);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            //paging & sorting
            model.Page = Page;
            model.Size = Size;
            var TotalPage = (int)Math.Ceiling((double)List.Count() / Size);
            model.haveNext = Page >= TotalPage ? false : true;
            model.havePrevious = Page <= 1 ? false : true;

            List = List.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.OrderCode)
                            .Skip((Page - 1) * Size).Take(Size);

            foreach (var v in List)
            {
                var ProductList = new List<StoreOrderProductVM>();
                foreach (var op in OrderProductProvider.GetAllByOrderId(v.Id))
                {
                    ProductList.Add(new StoreOrderProductVM()
                    {
                        OrderProductId = op.Id,
                        Price = (int)op.Price,
                        Quantity = op.Quantity ?? 0,
                        Name = op.Name,
                        Weight = op.Weight ?? 0,
                        ProductId = op.ProductId,
                        MainProductImage = (ProductImageProvider.GetMainImageByProductId(op.ProductId) ?? new PRODUCT_IMAGE()).URL,
                    });
                }

                model.OrderList.Add(new StoreOrderVM
                {
                    Id = v.Id,
                    CreatedAt = v.CreatedAt,
                    Address = v.Address,
                    ContactNo = v.ContactNo,
                    ExpirationDate = v.ExpirationDate,
                    OrderCode = v.OrderCode,
                    Receiver = v.Receiver,
                    Sender = v.Sender,
                    ShippingFee = v.ShippingFee,
                    TotalPrice = v.TotalPrice - v.AdditionalDiscount,
                    GrandTotal = v.GrandTotal,
                    TotalWeight = v.TotalWeight,
                    TrackingNo = v.TrackingNo,
                    Status = v.Status,
                    PaymentMethod = v.PaymentMethod,
                    Shipping = v.Shipping,
                    ShippingDate = v.ShippingDate,
                    City = v.City,
                    Subdistrict = v.Subdistrict,

                    ProductList = ProductList,
                    ReceivedPaymentConfirmation = v.ReceivedPaymentConfirmationAt
                });

            }

            return View(model);
        }

        [HttpPost]
        public JsonResult AddToCart(long ProductId, int Quantity)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var Product = ProductBLL.Get(ProductId, Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);

                if (Product.Stock < Quantity)
                {
                    Quantity = Product.Stock ?? 0;
                }

                var ShoppingCartVO = new SHOPPING_CART()
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                };

                if (Session["User_ID"] != null)
                {
                    var UserId = (long)Session["User_ID"];

                    ShoppingCartVO.UserId = UserId;

                    var SameProductInCart = ShoppingCartProvider.Get(UserId, ProductId);
                    if (SameProductInCart != null)
                    {
                        ShoppingCartVO = SameProductInCart;

                        ShoppingCartVO.Quantity = ShoppingCartVO.Quantity + Quantity;

                        //if more than stock
                        ShoppingCartVO.Quantity = ShoppingCartVO.Quantity > Product.Stock ? Product.Stock : ShoppingCartVO.Quantity;

                        ShoppingCartProvider.Edit(ShoppingCartVO, UserId);
                    }
                    else
                    {
                        ShoppingCartProvider.Add(ShoppingCartVO, UserId);
                    }
                }
                else
                {
                    var GuestId = Request.AnonymousID;

                    ShoppingCartVO.GuestID = GuestId;

                    var SameProductInCart = ShoppingCartProvider.Get(GuestId, ProductId);
                    if (SameProductInCart != null)
                    {
                        ShoppingCartVO = SameProductInCart;

                        ShoppingCartVO.Quantity = ShoppingCartVO.Quantity + Quantity;

                        //if more than stock
                        ShoppingCartVO.Quantity = ShoppingCartVO.Quantity > Product.Stock ? Product.Stock : ShoppingCartVO.Quantity;

                        ShoppingCartProvider.Edit(ShoppingCartVO, 0);
                    }
                    else
                    {
                        ShoppingCartProvider.Add(ShoppingCartVO, 0);
                    }
                }

                result.Success = true;

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        public JsonResult GetCart()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var Cart = ShoppingCartProvider.GetAll();

                if (Session["User_ID"] != null)
                {
                    var UserId = (long)Session["User_ID"];

                    Cart = Cart.Where(m => m.UserId == UserId);
                }
                else
                {
                    var GuestId = Request.AnonymousID;

                    Cart = Cart.Where(m => m.GuestID == GuestId);
                }

                var ShoppingCartList = new List<CartItem>();
                foreach (var v in Cart)
                {
                    ShoppingCartList.Add(new CartItem()
                    {
                        ProductId = v.ProductId ?? 0,
                        Quantity = v.Quantity ?? 0
                    });
                }
                result.Result = ShoppingCartList;

                result.Success = true;

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartCount()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var Cart = ShoppingCartProvider.GetAll();

                if (Session["User_ID"] != null)
                {
                    var UserId = (long)Session["User_ID"];

                    Cart = Cart.Where(m => m.UserId == UserId);
                }
                else
                {
                    var GuestId = Request.AnonymousID;

                    Cart = Cart.Where(m => m.GuestID == GuestId);
                }

                result.Result = Cart.Sum(m => m.Quantity);
                if (Cart.Count() == 0)
                {
                    result.Result = 0;
                }

                result.Success = true;

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ShoppingCartVM PopulateShoppingCartVM()
        {
            var model = new ShoppingCartVM();

            var Cart = ShoppingCartProvider.GetAll();

            if (Session["User_ID"] != null)
            {
                var UserId = (long)Session["User_ID"];

                Cart = Cart.Where(m => m.UserId == UserId);
            }
            else
            {
                var GuestId = Request.AnonymousID;

                Cart = Cart.Where(m => m.GuestID == GuestId);
            }

            foreach (var v in Cart)
            {
                var p = ProductBLL.Get(v.ProductId ?? 0, Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);

                model.ItemList.Add(new CartItem()
                {
                    CartId = v.Id,
                    ProductId = v.ProductId ?? 0,
                    Quantity = v.Quantity ?? 0,
                    isStockInsufficient = v.Quantity > p.Stock,
                    ProductDetails = new ProductDetailsPageVM()
                    {
                        Id = p.Id,
                        Description = p.Description,
                        Name = p.Name,
                        Price = (int)p.Price,
                        Sales = p.Sales ?? false,
                        SalesDiscount = (int)(p.SalesDiscount ?? 0),
                        TotalPrice = (int)((p.Price ?? 0) - (p.Sales == true ? (p.SalesDiscount ?? 0) : 0)),
                        ProductCode = p.ProductCode,
                        Size = p.Size,
                        Stock = p.Stock,
                        Weight = p.Weight,
                        Point = p.Point ?? 0,

                        ProductImageList = ProductImageProvider.GetAllByProductId(p.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList()
                    }
                });

                model.TotalPrice += (v.Quantity ?? 0) * (int)((p.Price ?? 0) - (p.Sales == true ? (p.SalesDiscount ?? 0) : 0));
                model.TotalWeight += (v.Quantity ?? 0) * (p.Weight ?? 0);
            }

            var TotalQuantity = model.ItemList.Sum(m => m.Quantity);
            if (TotalQuantity >= 8)
            {
                model.QuantityDiscount = TotalQuantity * 5000;
            }

            return model;
        }

        public ActionResult ShoppingCart()
        {
            var model = PopulateShoppingCartVM();

            return View(model);
        }

        public ActionResult _UpdateCartItem(long CartId)
        {
            var model = new CartItem();

            var VO = ShoppingCartProvider.Get(CartId);

            model.CartId = VO.Id;
            model.ProductId = VO.ProductId ?? 0;
            model.Quantity = VO.Quantity ?? 0;

            var p = ProductBLL.Get(model.ProductId, Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);
            model.ProductDetails = new ProductDetailsPageVM()
            {
                Id = p.Id,
                Description = p.Description,
                Name = p.Name,
                Price = (int)p.Price,
                Sales = p.Sales ?? false,
                SalesDiscount = (int)(p.SalesDiscount ?? 0),
                TotalPrice = (int)((p.Price ?? 0) - (p.Sales == true ? (p.SalesDiscount ?? 0) : 0)),
                ProductCode = p.ProductCode,
                Size = p.Size,
                Stock = p.Stock,
                Weight = p.Weight,

                ProductImageList = ProductImageProvider.GetAllByProductId(p.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList()
            };
            model.isStockInsufficient = model.Quantity > p.Stock;

            return View(model);
        }

        [HttpPost]
        public ActionResult _UpdateCartItem(CartItem Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];

                    var VO = ShoppingCartProvider.Get(Model.CartId);

                    VO.Quantity = Model.Quantity;

                    ShoppingCartProvider.Edit(VO, UserId);

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

            var p = ProductBLL.Get(Model.ProductId, Session["User_ID"] == null ? 0 : (long)Session["User_ID"]);
            Model.ProductDetails = new ProductDetailsPageVM()
            {
                Id = p.Id,
                Description = p.Description,
                Name = p.Name,
                Price = (int)p.Price,
                Sales = p.Sales ?? false,
                SalesDiscount = (int)(p.SalesDiscount ?? 0),
                TotalPrice = (int)((p.Price ?? 0) - (p.Sales == true ? (p.SalesDiscount ?? 0) : 0)),
                ProductCode = p.ProductCode,
                Size = p.Size,
                Stock = p.Stock,
                Weight = p.Weight,

                ProductImageList = ProductImageProvider.GetAllByProductId(p.Id).Select(m => new ProductImageVM() { Id = m.Id, URL = m.URL, DisplayOrder = m.DisplayOrder ?? 0 }).ToList()
            };
            Model.isStockInsufficient = Model.Quantity > p.Stock;

            return View(Model);
        }

        public ActionResult RemoveCartItem(long CartId)
        {
            ShoppingCartProvider.Delete(CartId);

            return RedirectToAction("ShoppingCart", "Home");
        }

        public ActionResult _AddAddress()
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new AddressBookVM();

            return View(model);
        }

        [HttpPost]
        public ActionResult _AddAddress(AddressBookVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];

                    var VO = new ADDRESS_BOOK();
                    VO.Address = Model.Address;
                    VO.AddressName = Model.AddressName;
                    VO.City = Model.City;
                    VO.Phone = Model.Phone;
                    VO.Postcode = Model.Postcode;
                    VO.Receiver = Model.Receiver;
                    VO.Subdistrict = Model.Subdistrict;
                    VO.SubdistrictId = Model.SubdistrictId;

                    VO.UserId = UserId;

                    AddressBookProvider.Add(VO, UserId);

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
        public ActionResult _EditAddress(long Id)
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var AddressVO = AddressBookProvider.Get(Id);

            var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];
            if (AddressVO.UserId != UserId)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new AddressBookVM();

            model.Address = AddressVO.Address;
            model.AddressName = AddressVO.AddressName;
            model.City = AddressVO.City;
            model.Id = AddressVO.Id;
            model.Phone = AddressVO.Phone;
            model.Postcode = AddressVO.Postcode;
            model.Receiver = AddressVO.Receiver;
            model.Subdistrict = AddressVO.Subdistrict;
            model.SubdistrictId = AddressVO.SubdistrictId;

            return View(model);
        }

        [HttpPost]
        public ActionResult _EditAddress(AddressBookVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];

                    var VO = AddressBookProvider.Get(Model.Id);
                    VO.Address = Model.Address;
                    VO.AddressName = Model.AddressName;
                    VO.City = Model.City;
                    VO.Phone = Model.Phone;
                    VO.Postcode = Model.Postcode;
                    VO.Receiver = Model.Receiver;
                    VO.Subdistrict = Model.Subdistrict;
                    VO.SubdistrictId = Model.SubdistrictId;
                    
                    AddressBookProvider.Edit(VO, UserId);

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

        public ActionResult _DeleteAddress(long Id)
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var AddressVO = AddressBookProvider.Get(Id);

            var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];
            if (AddressVO.UserId != UserId)
            {
                return RedirectToAction("Index", "Home");
            }

            AddressBookProvider.Delete(AddressVO.Id);
            
            if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
            {
                return Redirect(Url.Action("Index", "Home"));
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult _TopUp()
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new BalanceTopUpVM();
            model.TransferDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return View(model);
        }

        [HttpPost]
        public ActionResult _TopUp(BalanceTopUpVM Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime? TransferDateTime = null;
                    if (!string.IsNullOrWhiteSpace(Model.TransferDateTime))
                    {
                        try
                        {
                            TransferDateTime = DateTime.ParseExact(Model.TransferDateTime, "dd/MM/yyyy HH:mm", null);
                        }
                        catch
                        {
                            throw new Exception("'" + Model.TransferDateTime + "' is not valid.");
                        }
                    }

                    var UserId = Session["User_ID"] == null ? 0 : (long)Session["User_ID"];

                    var VO = new USER_BALANCE_TOPUP();
                    VO.AccountName = Model.AccountName;
                    VO.Amount = Model.Amount;
                    VO.Bank = Model.Bank;
                    VO.TransferDateTime = TransferDateTime;

                    VO.UserId = UserId;
                    VO.Status = TOP_UP_STATUS.PENDING;

                    UserBalanceTopUpProvider.Add(VO, UserId);

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

        public JsonResult GetAddressBook()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var UserId = (long)Session["User_ID"];

                var AddressBooks = new List<AddressBookVM>();

                foreach (var v in new AddressBookProvider().GetAll(UserId))
                {
                    AddressBooks.Add(new AddressBookVM()
                    {
                        Id = v.Id,
                        Address = v.Address,
                        AddressName = v.AddressName,
                        City = v.City,
                        Phone = v.Phone,
                        Postcode = v.Postcode,
                        Receiver = v.Receiver,
                        Subdistrict = v.Subdistrict,
                        SubdistrictId = v.SubdistrictId ?? 0,
                    });
                }

                result.Result = AddressBooks;

                result.Success = true;

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }

            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckOut()
        {
            var model = new CheckOutVM();

            model.Cart = PopulateShoppingCartVM();

            if (model.Cart.ItemList.Count() <= 0 || model.Cart.ItemList.Where(m => m.isStockInsufficient == true).Count() > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Session["User_ID"] != null)
            {
                ViewBag.Balance = UserProvider.Get((long)Session["User_ID"]).Balance ?? 0;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckOut(CheckOutVM Model, FormCollection form)
        {
            Model.Cart = PopulateShoppingCartVM();
            if (Session["User_ID"] != null)
            {
                ViewBag.Balance = UserProvider.Get((long)Session["User_ID"]).Balance ?? 0;
            }

            var ProductProvider = new ProductProvider();

            if (ModelState.IsValid)
            {
                //fill BalanceAmountToUse if isPayWithBalance == true
                if (Model.isPayWithBalance == true && Session["User_ID"] != null)
                {
                    Model.BalanceAmountToUse = (int)(UserProvider.Get((long)Session["User_ID"]).Balance ?? 0);
                    //this BalanceAmountToUse will be checked on below
                }

                if (Model.Cart.ItemList.Count() <= 0 || Model.Cart.ItemList.Where(m => m.isStockInsufficient == true).Count() > 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                //manual input shipping fee
                double? ManualInputShippingFee = null;
                if (Constant.ManualFeeShippingList.Contains(Model.Shipping))
                {
                    ManualInputShippingFee = double.Parse(form[Model.Shipping + " fare"]);
                }
                else if(Constant.FixedFeeShippingList.ContainsKey(Model.Shipping))
                {
                    ManualInputShippingFee = Constant.FixedFeeShippingList[Model.Shipping];
                }

                if (Session["User_ID"] != null)
                {
                    double ShippingFee;
                    if (ManualInputShippingFee != null)
                    {
                        ShippingFee = ManualInputShippingFee.Value;
                    }
                    else
                    {
                        ShippingFee = RajaOngkir.GetShippingFee(Model.SubdistrictId.ToString(), Model.Shipping).FirstOrDefault().costs.Where(m => m.service == Model.ShippingServiceCode).FirstOrDefault().cost.FirstOrDefault().value;
                        var ShippingWeight = (Model.Cart.TotalWeight - Constant.ShippingWeightTolerance) <= 0 ? Constant.ShippingWeightTolerance : (Model.Cart.TotalWeight - Constant.ShippingWeightTolerance);
                        ShippingFee *= Math.Ceiling((double)ShippingWeight / 1000);
                    }

                    var TotalPayment = (Model.Cart.TotalPrice - Model.Cart.QuantityDiscount + ShippingFee);

                    //checking "BalanceAmountToUse" validity
                    ///  checking if "BalanceAmountToUse" exceed "Total Payment (TotalPrice + ShippingFee)"
                    Model.BalanceAmountToUse = Model.BalanceAmountToUse > TotalPayment ? (int)TotalPayment : Model.BalanceAmountToUse;
                    ///  checking if "BalanceAmountToUse" exceed User "Balance" 
                    var Balance = UserProvider.Get((long)Session["User_ID"]).Balance ?? 0;
                    Model.BalanceAmountToUse = Model.BalanceAmountToUse > Balance ? (int)Balance : Model.BalanceAmountToUse;

                    var UserPaymentAmount = TotalPayment - Model.BalanceAmountToUse;

                    if (UserPaymentAmount <= 0)
                    {
                        var NewOrderId = CheckOutProcess(Model, new CheckOutPaymentVM()
                        {
                            BankAccountName = "",
                            PaymentMethod = "Balance"
                        },
                        ManualInputShippingFee);

                        return RedirectToAction("CheckOutSuccess", "Home", new { Id = NewOrderId });
                    }
                }

                TempData["CheckOutDetails"] = Model;
                TempData["ManualInputShippingFee"] = ManualInputShippingFee;

                return RedirectToAction("CheckOutPayment", "Home");
            }
            else
            {
                TempData["Message"] = string.Join(" ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return View(Model);
        }

        public ActionResult CheckOutPayment()
        {
            if (TempData["CheckOutDetails"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["CheckOutDetails"] = TempData["CheckOutDetails"];
            TempData["ManualInputShippingFee"] = TempData["ManualInputShippingFee"];

            var model = new CheckOutPaymentVM();

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckOutPayment(CheckOutPaymentVM Model, FormCollection form)
        {
            if (TempData["CheckOutDetails"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var CheckOutVM = TempData["CheckOutDetails"] as CheckOutVM;
                    var ManualInputShippingFee = TempData["ManualInputShippingFee"] as double?;

                    var NewOrderId = CheckOutProcess(CheckOutVM, Model, ManualInputShippingFee);

                    return RedirectToAction("CheckOutSuccess", "Home", new { Id = NewOrderId });
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

        [NonAction]
        private long CheckOutProcess(CheckOutVM CheckOut, CheckOutPaymentVM Payment, double? ManualInputShippingFee)
        {
            CheckOut.Cart = PopulateShoppingCartVM();

            long UserId = 0;
            if (Session["User_ID"] != null)
            {
                UserId = (long)Session["User_ID"];
            }
            else
            {
                var AdminId = new AdminProvider().GetAllAdminByWebsite("b21c").Select(m => m.Id).First(); //getting the first admin id (ugly)
                UserId = AdminId;
            }

            double ShippingFee;
            if (Constant.ManualFeeShippingList.Contains(CheckOut.Shipping) || Constant.FixedFeeShippingList.ContainsKey(CheckOut.Shipping))
            {
                ShippingFee = ManualInputShippingFee.Value;
            }
            else
            {
                ShippingFee = RajaOngkir.GetShippingFee(CheckOut.SubdistrictId.ToString(), CheckOut.Shipping).FirstOrDefault().costs.Where(m => m.service == CheckOut.ShippingServiceCode).FirstOrDefault().cost.FirstOrDefault().value;
                var ShippingWeight = (CheckOut.Cart.TotalWeight - Constant.ShippingWeightTolerance) <= 0 ? Constant.ShippingWeightTolerance : (CheckOut.Cart.TotalWeight - Constant.ShippingWeightTolerance);
                ShippingFee *= Math.Ceiling((double)ShippingWeight / 1000);
            }

            var OrderVO = new ORDER();
            if (Session["User_ID"] != null)
                OrderVO.BookedBy = new UserProvider().Get(UserId).Name;
            else
                OrderVO.BookedBy = "Guest Buyer";
            OrderVO.Sender = string.IsNullOrWhiteSpace(CheckOut.Sender) ? "Bags 21 Century" : CheckOut.Sender;
            OrderVO.Receiver = CheckOut.Receiver;
            OrderVO.Address = CheckOut.Address + ", " + CheckOut.Subdistrict + ", " + CheckOut.City + " - " + CheckOut.Postcode;
            OrderVO.City = CheckOut.City;
            OrderVO.Subdistrict = CheckOut.Subdistrict;
            OrderVO.SubdistrictId = CheckOut.SubdistrictId;
            OrderVO.ContactNo = CheckOut.ContactNo;
            OrderVO.Shipping = CheckOut.Shipping + (CheckOut.ShippingServiceCode == "manual" ? "" : " " + CheckOut.ShippingServiceCode);
            OrderVO.ShippingFee = (decimal)ShippingFee;
            OrderVO.ShippingCode = OrderVO.Shipping.ToUpper() + "-" + (int)(ShippingFee / 1000);
            OrderVO.Status = ORDER_STATUS_CODE.PENDING_PAYMENT.ToString();
            OrderVO.ExpirationDate = DateTime.Now.AddDays(2);

            OrderVO.PaymentMethod = Payment.PaymentMethod;
            OrderVO.BankAccountName = Payment.BankAccountName;

            OrderVO.TotalPrice = CheckOut.Cart.TotalPrice;
            OrderVO.AdditionalDiscount = CheckOut.Cart.QuantityDiscount;
            OrderVO.TotalWeight = CheckOut.Cart.TotalWeight;

            OrderVO.Discount = 0; /////

            OrderVO.Note = CheckOut.Note;

            OrderVO.FlgAdminLogo = false;

            if (Session["User_ID"] != null)
            {
                var TotalPayment = ((double)(OrderVO.TotalPrice - OrderVO.AdditionalDiscount) + ShippingFee);
                //checking "BalanceAmountToUse" validity
                ///  checking if "BalanceAmountToUse" exceed "Total Payment (TotalPrice + ShippingFee)"
                CheckOut.BalanceAmountToUse = CheckOut.BalanceAmountToUse > TotalPayment ? (int)TotalPayment : CheckOut.BalanceAmountToUse;
                ///  checking if "BalanceAmountToUse" exceed User "Balance" 
                var Balance = UserProvider.Get((long)Session["User_ID"]).Balance ?? 0;
                CheckOut.BalanceAmountToUse = CheckOut.BalanceAmountToUse > Balance ? (int)Balance : CheckOut.BalanceAmountToUse;

                OrderVO.UserPaymentAmount = (OrderVO.TotalPrice - OrderVO.AdditionalDiscount) + OrderVO.ShippingFee - CheckOut.BalanceAmountToUse;
            }
            else
            {
                OrderVO.UserPaymentAmount = (OrderVO.TotalPrice - OrderVO.AdditionalDiscount) + OrderVO.ShippingFee;
            }
            if (OrderVO.UserPaymentAmount <= 0)
            {
                OrderVO.Status = ORDER_STATUS_CODE.PREPARE_SHIPMENT.ToString();
                OrderVO.PrepareShipmentDate = DateTime.Now;
            }

            OrderVO.PackingCode = "A";

            var OrderId = OrderProvider.Add(OrderVO, UserId);

            if (Session["User_ID"] != null && CheckOut.BalanceAmountToUse != 0)
            { 
                var OrderCode = OrderProvider.Get(OrderId).OrderCode;

                //cut User Balance
                UserProvider.AddBalance(UserId, (CheckOut.BalanceAmountToUse * -1), "Use as Payment for Order : " + OrderCode, UserId);
            }

            //{ Add Products
            //populate OrderProductList
            List<ORDER_PRODUCT> OrderProductList = new List<ORDER_PRODUCT>();
            foreach (var v in CheckOut.Cart.ItemList)
            {
                var Product = ProductBLL.Get(v.ProductId);
                OrderProductList.Add(new ORDER_PRODUCT()
                {
                    ProductId = v.ProductId,
                    Quantity = v.Quantity,
                    Name = v.ProductDetails.Name,
                    Price = v.ProductDetails.TotalPrice,
                    Weight = v.ProductDetails.Weight,
                    Point = v.ProductDetails.Point,
                    OrderId = OrderId,
                    Color = Product.Color,
                    Modal = Product.ModalPrice,
                    ProductCode = Product.ProductCode,
                });
            }

            var ProductProvider = new ProductProvider();

            //check if Selected Product's stock availability
            if (ProductProvider.CheckStockAvailability(OrderProductList) == false)
            {
                throw new Exception("Insufficient stock for selected product.");
            }

            //add Order Product
            foreach (var v in OrderProductList)
            {
                OrderProductProvider.Add(v, UserId);
            }

            //cut Product's stock value
            foreach (var v in OrderProductList)
            {
                var ProductVO = ProductProvider.Get(v.ProductId);
                ProductVO.Stock = ProductVO.Stock - v.Quantity;

                ProductProvider.Edit(ProductVO, UserId, PRODUCT_MODIFICATION_ACTION.BOOK);
            }
            // }

            //clear Cart
            if (Session["User_ID"] != null)
            {
                UserId = (long)Session["User_ID"];
                ShoppingCartProvider.DeleteByUserId(UserId);
            }
            else
            {
                var GuestId = Request.AnonymousID;
                ShoppingCartProvider.DeleteByGuestID(GuestId);
            }

            return OrderId;
        }

        public ActionResult CheckOutSuccess(long Id)
        {
            var OrderVO = OrderProvider.Get(Id);

            var model = new CheckOutSuccessVM()
            {
                OrderCode = OrderVO.OrderCode,
                PaymentMethod = OrderVO.PaymentMethod,
                PaymentAmount = (int)(OrderVO.UserPaymentAmount),
                ExpirationDate = OrderVO.ExpirationDate.Value
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult SetSurveyAsCompleted()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var UserId = (long)Session["User_ID"];
                var SurveyId = (long)Session["User_SurveyID"];

                new UserSurveyProvider().Add(new USER_SURVEY()
                {
                    SurveyId = SurveyId,
                    UserId = UserId,
                }, UserId);

                Session.Remove("User_SurveyID");
                Session.Remove("User_SurveyURL");

                result.Result = "";

                result.Success = true;

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        public ActionResult CancelOrder(long OrderId)
        {
            var UserId = (long)Session["User_ID"];

            var OrderVO = OrderProvider.Get(OrderId);

            if (OrderVO.Status != ORDER_STATUS_CODE.PENDING_PAYMENT
                 && OrderVO.CreatedBy != UserId)
            {
            }
            else
            {
                OrderVO.Status = ORDER_STATUS_CODE.CANCELED;

                OrderProvider.Edit(OrderVO, UserId);

                //add back Product's stock value
                var Order_ProductList = OrderProductProvider.GetAllByOrderId(OrderId);
                foreach (var v in Order_ProductList.GroupBy(m => m.ProductId).Select(m => new { Id = m.Key, Count = m.Sum(n => n.Quantity) }))
                {
                    var ProductVO = new ProductProvider().Get(v.Id);

                    ProductVO.Stock = ProductVO.Stock + v.Count;

                    new ProductProvider().Edit(ProductVO, UserId, PRODUCT_MODIFICATION_ACTION.CANCEL);
                }

                //add back the balance user used
                var UserBalanceTrx = new UserBalanceTrxProvider().GetAll("b21c").Where(m => m.Remarks.Contains("Use as Payment for Order : " + OrderVO.OrderCode)).FirstOrDefault();
                if (UserBalanceTrx != null)
                {
                    UserProvider.AddBalance(UserBalanceTrx.UserId ?? 0, (UserBalanceTrx.Amount ?? 0) * -1, "Cancel Order : " + OrderVO.OrderCode, UserId);
                }
            }

            if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult DeliveredOrder(long OrderId)
        {
            var UserId = (long)Session["User_ID"];

            var OrderVO = OrderProvider.Get(OrderId);

            if (OrderVO.Status != ORDER_STATUS_CODE.SHIPPED
                && OrderVO.CreatedBy != UserId)
            {
            }
            else
            {
                OrderVO.Status = ORDER_STATUS_CODE.DELIVERED;

                OrderProvider.Edit(OrderVO, UserId);

                //add RESELLER Point (one time)
                var Buyer = UserProvider.Get(OrderVO.CreatedBy.Value);
                if (Buyer != null && Buyer.Type == USER_TYPE.RESELLER)
                {
                    var AddPointAmount = 0;
                    foreach (var v in OrderProductProvider.GetAllByOrderId(OrderVO.Id))
                    {
                        AddPointAmount += ((v.Quantity ?? 0) * (v.Point ?? 0));
                    }
                    UserProvider.AddPoint(OrderVO.CreatedBy.Value, AddPointAmount, UserId);
                }
            }

            if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult SendPaymentConfirmation(long OrderId)
        {
            var UserId = (long)Session["User_ID"];

            var OrderVO = OrderProvider.Get(OrderId);

            if (OrderVO.Status != ORDER_STATUS_CODE.PENDING_PAYMENT || OrderVO.ReceivedPaymentConfirmationAt != null
                 || OrderVO.CreatedBy != UserId)
            {
            }
            else
            {
                OrderVO.ReceivedPaymentConfirmationAt = DateTime.Now;

                OrderProvider.Edit(OrderVO, UserId);
            }

            if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult Help()
        {
            ViewBag.Lookup = LookupProvider.Get("Help").Value;

            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult MemberStatus()
        {
            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var UserId = (long)Session["User_ID"];

            var UserVO = new UserProvider().Get(UserId);

            var model = new MemberStatusVM()
            {
                BirthDate = UserVO.BirthDate == null ? null : UserVO.BirthDate.Value.ToString("dd/MM/yyyy"),
                Email = UserVO.Username,
                Gender = UserVO.Gender,
                Name = UserVO.Name,
                Phone = UserVO.Phone,
                Balance = (int)(UserVO.Balance ?? 0),
                Point = UserVO.Point ?? 0,
                MemberStatus = UserVO.USER_STATUS.Name,
            };

            foreach (var v in new UserStatusProvider().GetAll().Where(m => !m.Name.Contains("partner")))
            {
                model.StatusList.Add(new UserStatusVM()
                {
                    Id = v.Id,
                    Discount = (int)(v.Discount ?? 0),
                    IconImage = v.IconImage,
                    Name = v.Name,
                    RegistrationFee = (int)(v.RegistrationFee ?? 0),
                    PointNeeded = v.PointNeeded
                });
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ChangeMemberStatus(long StatusId)
        {
            AjaxResult result = new AjaxResult();

            //if not logged in, redirect to home
            if (Session["User_ID"] == null)
            {
                return Json(new AjaxResult()
                {
                     Success = false,
                     Message = "Mohon log in terlebih dahulu."
                });
            }

            var UserId = (long)Session["User_ID"];

            try
            {
                var UserVO = new UserProvider().Get(UserId);
                var UserStatusVO = new UserStatusProvider().Get(StatusId);

                if ((UserVO.Balance ?? 0) >= (UserStatusVO.RegistrationFee.Value))
                {
                    UserVO.UserStatusId = UserStatusVO.Id;
                    UserVO.USER_STATUS = null; //has to set to null because 'Get' Method got Include this USER_STATUS

                    //change status also change Point
                    UserVO.Point = UserStatusVO.PointNeeded;

                    UserProvider.Edit(UserVO, UserId);
                    
                    UserProvider.UpdateBalance(UserId, (UserVO.Balance ?? 0) - (UserStatusVO.RegistrationFee.Value), "Payment for changing User Status to " + UserStatusVO.Name, UserId);

                    result.Result = true;
                    result.Success = true;
                }
                else
                {
                    result.Message = "Saldo tidak mencukupi.";
                }

                if (Request.UrlReferrer == null || string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                {
                    result.ReturnURL = Url.Action("Index", "Home");
                }
                else
                {
                    result.ReturnURL = Request.UrlReferrer.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }


    }
}