using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Models
{
    public class UserBalanceTopUpVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public string Status { get; set; }
        public string AccountName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Bank { get; set; }
        public Nullable<System.DateTime> TransferDateTime { get; set; }
        public Nullable<long> UserId { get; set; }

        public string Username { get; set; }
        public string UserName { get; set; }
    }
}