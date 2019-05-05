using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace B21C.Helper.Constants
{
    public class Constant
    {
        public static string UploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public static int ShippingWeightTolerance = 400; //gram

        public static int AdditionalDiscount_PerProduct = 55000;
        public static int AdditionalDiscount_PerDiscountedProduct = 10000;

        public static List<string> ManualFeeShippingList = new List<string>()
        {
            "Lion Parcel",
            "JNT Shopee",
            "COD",
            "Others"
        };
        public static Dictionary<string, double> FixedFeeShippingList = new Dictionary<string, double>()
        {
            { "COD", 0 }
        };
    }
}