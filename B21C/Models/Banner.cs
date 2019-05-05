using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class BannerVM
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LastUpdAt { get; set; }
        public string Status { get; set; }
        public string URL { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImageURL { get; set; }
    }
}