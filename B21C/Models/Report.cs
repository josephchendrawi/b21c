using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class OverallProductReportVM
    {
        [Display(Name = "Total Current Stock")]
        public int TotalCurrentStock { get; set; }
        [Display(Name = "Total Price of Current Stock")]
        public decimal TotalCurrentStockPrice { get; set; }
    }

    public class OrderProductReportItem
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<int> TotalWeight { get; set; }
        public string OrderCode { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string BookedBy { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public Nullable<bool> FlgAdminLogo { get; set; }
        public string TrackingNo { get; set; }
        public string PaymentMethod { get; set; }
        public string Shipping { get; set; }
        public string ShippingCode { get; set; }
        public Nullable<decimal> DiscountForEachProduct { get; set; }
        public Nullable<System.DateTime> PrepareShipmentDate { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> AdditionalDiscount { get; set; }
        public Nullable<System.DateTime> ShippingDate { get; set; }

        public List<OrderProductVM_forOrderProductReport> ProductList { get; set; }
        public int ProductCount { get; set; }
        public Nullable<decimal> TotalModalPrice { get; set; }

        public string CreatedBy { get; set; }
        public decimal TotalPartnerDiscount { get; set; }

        public string ProductCodeList { get; set; }
        //public Nullable<decimal> TotalDiscountForEachProduct { get; set; }
    }

    public class OrderProductVM_forOrderProductReport : OrderProductVM
    {
        public string ProductCode { get; set; }
        public decimal ModalPrice { get; set; }
    }

    public enum PackingShift
    {
        [Description("22:00-13:59")]
        Afternoon = 1,
        [Description("14:00-21:59")]
        Evening = 2,
    }

    public class PackingShiftReportVM
    {
        public DateTime Date { get; set; }
        public PackingShift Shift { get; set; }
    }

    public class OrderProductVM_forPackingShiftReport : OrderProductVM
    {
        public string ProductCode { get; set; }
        public DateTime? PrepareShipmentDate { get; set; }
        public string OrderCode { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Address { get; set; }
    }

    public class ShippingReportItem
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime ShippingDate { get; set; }
        public string TrackingNo { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ShippingMethod { get; set; }
        public int ShippingFee { get; set; }

        public int TrackingNoCount { get; set; }
    }

    public class ShippingReportVM
    {
        public string Note { get; set; }
    }

    public class Partner_OrderReportVM : OrderVM
    {
        public decimal TotalPartnerDiscount { get; set; }
    }

}