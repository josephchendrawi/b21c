using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class ProductStockLogVM
    {
        public ProductStockLogVM()
        {
            Available_Products = new List<string>();
            SoldOut_Products = new List<string>();
            Canceled_Products = new List<string>();
            
            Addition_SoldOut_Products = new List<string>();
        }

        public int Available_Amount { get; set; }
        public int SoldOut_Amount { get; set; }
        public int Canceled_Amount { get; set; }

        public List<string> Available_Products { get; set; }
        public List<string> SoldOut_Products { get; set; }
        public List<string> Canceled_Products { get; set; }

        public DateTime Date { get; set; }

        public DateTime? CompareDate { get; set; }
        public List<string> Addition_SoldOut_Products { get; set; }
    }

}