using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class HomepageVM
    {
        public HomepageVM()
        {
            BannerList = new List<BannerVM>();
            NewProductList = new List<ProductVM>();
            SalesProductList = new List<ProductVM>();
            CatalogList = new List<CatalogVM>();
            SpecialPromoProductList = new List<ProductVM>();
        }

        public List<BannerVM> BannerList { get; set; }
        public List<ProductVM> NewProductList { get; set; }
        public List<ProductVM> SalesProductList { get; set; }
        public List<ProductVM> SpecialPromoProductList { get; set; }

        public List<CatalogVM> CatalogList { get; set; }
    }
        
    public class ProductPageVM
    {
        public ProductPageVM()
        {
            ProductList = new List<ProductVM>();
            AvailablePage = new List<int>();
        }

        public List<ProductVM> ProductList { get; set; }

        public int Page { get; set; }
        public int Size { get; set; }
        public string Query { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public string WeightQuery { get; set; }

        public List<int> AvailablePage { get; set; }
        public bool haveNext { get; set; }
        public bool havePrevious { get; set; }
    }

    public class CategoryProductPageVM : ProductPageVM
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class ProductDetailsPageVM
    {
        public ProductDetailsPageVM()
        {
            ProductImageList = new List<ProductImageVM>();
        }

        public List<ProductImageVM> ProductImageList { get; set; }
        
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public Nullable<int> Price { get; set; }
        public string Size { get; set; }
        public Nullable<int> Weight { get; set; }
        public string Description { get; set; }
        public Nullable<int> Stock { get; set; }
        public bool Sales { get; set; }
        public Nullable<decimal> SalesDiscount { get; set; }
        public decimal TotalPrice { get; set; }
        public int Point { get; set; }

        public decimal MemberDiscount { get; set; }
    }

    public class SignupVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email Address not in the correct format")]

        public string Email { get; set; }
        [Display(Name = "Phone No.")]
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password length is incorrect (at least 6 characters)")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [CompareAttribute("Password", ErrorMessage = "Confirm Password does not match with Password")]
        public string ConfirmPassword { get; set; }
    }

    public class UserVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email Address not in the correct format")]

        public string Email { get; set; }
        [Display(Name = "Phone No.")]
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }

        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password length is incorrect (at least 6 characters)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [CompareAttribute("Password", ErrorMessage = "Confirm Password does not match with Password")]
        public string ConfirmPassword { get; set; }


        public int Balance { get; set; }
        public int Point { get; set; }
        public string MemberStatus { get; set; }
    }

    public class MyAccountVM : UserVM
    {
        public MyAccountVM()
        {
            AddressBookList = new List<AddressBookVM>();
        }

        public List<AddressBookVM> AddressBookList { get; set; }
        public string MemberStatusPhotoURL { get; set; }
    }

    public class CartItem
    {
        public long CartId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }

        public ProductDetailsPageVM ProductDetails { get; set; }

        public bool isStockInsufficient { get; set; }
    }

    public class ShoppingCartVM
    {
        public ShoppingCartVM()
        {
            ItemList = new List<CartItem>();
        }

        public List<CartItem> ItemList { get; set; }

        public int TotalPrice { get; set; }
        public int TotalWeight { get; set; }

        public int QuantityDiscount { get; set; }
    }

    public class BalanceTopUpVM
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public int? Amount { get; set; }
        [Required]
        public string Bank { get; set; }
        [Required]
        public string TransferDateTime { get; set; }
    }

    public class CheckOutVM
    {
        public ShoppingCartVM Cart { get; set; }

        public string Sender { get; set; }
        [Required]
        public string Receiver { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Postcode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Subdistrict { get; set; }
        [Required]
        public Nullable<int> SubdistrictId { get; set; }
        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string Shipping { get; set; }
        [Required]
        public string ShippingServiceCode { get; set; }

        [Required]
        public int BalanceAmountToUse { get; set; }
        public bool isPayWithBalance { get; set; }

        public string Note { get; set; }
    }

    public class CheckOutPaymentVM
    {
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string BankAccountName { get; set; }
    }

    public class CheckOutSuccessVM
    {
        public string OrderCode { get; set; }
        public string PaymentMethod { get; set; }
        public int PaymentAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class MyOrderVM
    {
        public MyOrderVM()
        {
            OrderList = new List<StoreOrderVM>();
        }

        public List<StoreOrderVM> OrderList { get; set; }

        public int Page { get; set; }
        public int Size { get; set; }
        public bool haveNext { get; set; }
        public bool havePrevious { get; set; }
        public string Status { get; set; }
    }

    public class StoreOrderVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<int> TotalWeight { get; set; }
        public string OrderCode { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string TrackingNo { get; set; }
        public string PaymentMethod { get; set; }
        public string Shipping { get; set; }
        public Nullable<System.DateTime> ShippingDate { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string Subdistrict { get; set; }

        public List<StoreOrderProductVM> ProductList { get; set; }

        public DateTime? ReceivedPaymentConfirmation { get; set; }
    }

    public class StoreOrderProductVM : OrderProductVM
    {
        public string MainProductImage { get; set; }
    }

    public class MemberStatusVM : UserVM
    {
        public MemberStatusVM()
        {
            StatusList = new List<UserStatusVM>();
        }

        public List<UserStatusVM> StatusList { get; set; }
    }

}