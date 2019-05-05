using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Models
{
    public class NewsletterVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public string Status { get; set; }

        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }

    public class SendNewsletterVM
    {
        public long Id { get; set; }

        public IEnumerable<String> Target { get; set; }
    }
}