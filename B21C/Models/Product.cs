using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class ProductVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }

        [Required]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Nullable<int> Price { get; set; }
        [Display(Name = "Modal")]
        public Nullable<decimal> ModalPrice { get; set; }
        public string Size { get; set; }
        [Required]
        public Nullable<int> Weight { get; set; }
        public string Description { get; set; }
        [Required]
        public Nullable<int> Stock { get; set; }
        [Display(Name = "Old Stock")]
        public Nullable<int> OldStock { get; set; }
        public bool Sales { get; set; }
        public Nullable<decimal> SalesDiscount { get; set; }
        [Required]
        public int Point { get; set; }

        public List<ProductImageVM> ProductImages { get; set; }
        public string MainProductImage { get; set; }

        public decimal TotalPrice { get; set; }

        public bool isNew { get; set; }

        [Required]
        [Display(Name = "Publish Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PublishAt { get; set; }


        public bool SpecialPromo { get; set; }

        [Display(Name = "Category")]
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class ProductImageVM
    {
        public long Id { get; set; }
        public string URL { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ProductStatusVM
    {
        public ProductStatusVM()
        {
            OrderStatusList = new Dictionary<string, int>();
        }

        public long ProductId { get; set; }
        public Dictionary<string, int> OrderStatusList { get; set; }

        public string LastReset_DT { get; set; }
    }

    public class ProductStockChangeVM
    {
        public long ProductId { get; set; }
        [Required]
        public int NewStockValue { get; set; }
        public string Remarks { get; set; }
    }

    public class ProductStockActivityVM
    {
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public string Remarks { get; set; }
    }

    public class ProductNewVM
    {
        public long ProductId { get; set; }
        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidFrom { get; set; }
        [Required]
        [Display(Name = "Valid To")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidTo { get; set; }
    }

}