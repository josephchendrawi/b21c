using B21C.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class OrderVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public string Status { get; set; }
        [Display(Name = "Shipping Fee")]
        public Nullable<decimal> ShippingFee { get; set; }
        [Display(Name = "Total Price")]
        public Nullable<decimal> TotalPrice { get; set; }
        [Display(Name = "Total Weight")]
        public Nullable<int> TotalWeight { get; set; }
        [Display(Name = "Order Code")]
        public string OrderCode { get; set; }
        [Display(Name = "Payment Expiration Date")]
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string BookedBy { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Address { get; set; }
        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }
        [Display(Name = "Use B21C Logo?")]
        public Nullable<bool> FlgAdminLogo { get; set; }
        [Display(Name = "Tracking No.")]
        public string TrackingNo { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Shipping { get; set; }
        public string ShippingCode { get; set; }
        public string PackingCode { get; set; }
        [Display(Name = "Discount for each Product")]
        public Nullable<decimal> Discount { get; set; }
        [Display(Name = "Additional Discount")]
        public Nullable<decimal> AdditionalDiscount { get; set; }
        [Display(Name = "Prepare Shipment Date")]
        public Nullable<System.DateTime> PrepareShipmentDate { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> ShippingDate { get; set; }
        [Display(Name = "Payment Code")]
        public int? PaymentCode { get; set; }

        [Display(Name = "Grand Total")]
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<decimal> TotalPriceWithAdditionalDiscount { get; set; }
        public bool PaymentExpired { get; set; }

        public List<OrderProductVM> ProductList { get; set; }

        [Display(Name = "Bank Account Name")]
        public string BankAccountName { get; set; }
        [Display(Name = "User Payment Amount")]
        public decimal UserPaymentAmount { get; set; }

        public string CreatedBy { get; set; }

        public decimal TotalPartnerDiscount { get; set; }

        public int PackedCount { get; set; }
    }

    public class OrderProductVM
    {
        public long OrderProductId { get; set; }
        public long ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Weight { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string ProductCode { get; set; }

        public long OrderId { get; set; }
        public string OrderCode { get; set; }
    }

    public class CreateOrUpdateOrderVM
    {
        [Required]
        public long Id { get; set; }
        [Required]
        [Display(Name = "Booked By")]
        public string BookedBy { get; set; }
        [Required]
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }
        [Display(Name = "Discount for each Product")]
        public decimal Discount { get; set; }
        [Display(Name = "Additional Discount")]
        public decimal AdditionalDiscount { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Required]
        public string Shipping { get; set; }

        public CreateOrUpdateOrderVM()
        {
            ProductList = new List<OrderProductVM>();
        }

        public List<OrderProductVM> ProductList { get; set; }
    }

    public class UpdateOrderProductVM : CreateOrUpdateOrderVM
    {
        public string AddedProductS { get; set; }
    }

    public class EditBookingVM
    {
        public EditBookingVM()
        {
            ProductList = new List<OrderProductVM>();
        }

        public long Id { get; set; }
        [Required]
        [Display(Name = "Booked By")]
        public string BookedBy { get; set; }
        [Required]
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }
        [Display(Name = "Additional Discount")]
        public decimal Discount { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Required]
        [Display(Name = "Total Weight")]
        public int TotalWeight { get; set; }
        [Required]
        public string Shipping { get; set; }

        public string ProductIds { get; set; }

        public List<OrderProductVM> ProductList { get; set; }
    }

    public class OrderInfoVM
    {
        public string BookedBy { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Shipping { get; set; }
        [Display(Name = "Total Weight")]
        public int TotalWeight { get; set; }
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }
        [Display(Name = "TotalPrice")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Additional Discount")]
        public decimal AdditionalDiscount { get; set; }
        [Display(Name = "Order Code")]
        public string OrderCode { get; set; }
        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }

        [Display(Name = "Bank Account Name")]
        public string BankAccountName { get; set; }
        [Display(Name = "User Payment Amount")]
        public decimal UserPaymentAmount { get; set; }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        [Display(Name = "Shipping Code")]
        public string ShippingCode { get; set; }
    }

    public class ShipOrderVM
    {
        public ShipOrderVM()
        {
            OrderInfo = new OrderInfoVM();
        }

        public OrderInfoVM OrderInfo { get; set; }

        [Required]
        public long Id { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Receiver { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }
        [Display(Name = "Use B21C Logo?")]
        public bool FlgAdminLogo { get; set; }
        [Required]
        [Display(Name = "Shipping Code")]
        public string ShippingCode { get; set; }
        [Required]
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }
        public string Note { get; set; }
        public string PackingCode { get; set; }
    }

    public class ShippedOrderVM
    {
        public ShippedOrderVM()
        {
            OrderInfo = new OrderInfoVM();
        }

        public OrderInfoVM OrderInfo { get; set; }

        [Required]
        public long Id { get; set; }
        [Display(Name = "Tracking No.")]
        public string TrackingNo { get; set; }
        [Required]
        [Display(Name = "Shipping Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ShippingDate { get; set; }
    }

    public class MutationCheckVM
    {
        public List<MutationCheck_OrderVM> MutationCheckOrderList { get; set; }
        public List<BankMutationItem> BCAMutation { get; set; }
        public List<BankMutationItem> MandiriMutation { get; set; }
        public List<BankMutationItem> BRIMutation { get; set; }
        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PrepareShipmentDate { get; set; }
    }

    public class MutationCheck_OrderVM : OrderVM
    {
        public string MatchedBank { get; set; }
    }

    public class PrintOrderVM
    {
        public PrintOrderVM()
        {
            ProductList = new List<OrderProductVM>();
        }

        public long OrderId { get; set; }
        public string ShippingCode { get; set; }
        public string OrderCode { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string PackingCode { get; set; }

        public List<OrderProductVM> ProductList { get; set; }
    }

}