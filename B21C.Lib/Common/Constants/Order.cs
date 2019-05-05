using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Common.Constants
{
    public class ORDER_STATUS_CODE
    {
        public static string PENDING_PAYMENT = "Pending Payment";
        public static string PREPARE_SHIPMENT = "Prepare Shipment";
        public static string SHIPPED = "Shipped";
        public static string DELIVERED = "Delivered";
        public static string RETURNED = "Returned";
        public static string CANCELED = "Canceled";
    }
}
