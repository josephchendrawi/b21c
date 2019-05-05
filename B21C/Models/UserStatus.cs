using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Models
{
    public class UserStatusVM
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Nullable<decimal> Discount { get; set; }
        [Required]
        [Display(Name = "Partner Discount")]
        public Nullable<decimal> PartnerDiscount { get; set; }
        [Required]
        [Display(Name = "Point Needed")]
        public Nullable<int> PointNeeded { get; set; }
        public string IconImage { get; set; }
        public decimal RegistrationFee { get; set; }
    }
}