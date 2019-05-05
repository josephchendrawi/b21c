using B21C.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Common.DTO
{
    public class ORDER_DTO : ORDER
    {
        public decimal? GrandTotal { get; set; }
        public decimal? TotalPriceWithAdditionalDiscount { get; set; }
        public bool PaymentExpired { get; set; }
        public string CreatedBy_Name { get; set; }
        public string CreatedBy_Type { get; set; }
        public string CreatedBy_Website { get; set; }
        public string CreatedBy_Role { get; set; }
    }
}
