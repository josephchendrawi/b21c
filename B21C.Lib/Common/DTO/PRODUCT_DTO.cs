using B21C.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Common.DTO
{
    public class PRODUCT_DTO : PRODUCT
    {
        public bool OutOfStock { get; set; }
        public bool isNew { get; set; }
    }
}
