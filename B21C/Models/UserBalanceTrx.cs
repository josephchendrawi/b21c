using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Models
{
    public class UserBalanceTrxVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> UserId { get; set; }
    }
}