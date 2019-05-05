using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class AddressBookVM
    {
        public long Id { get; set; }
        [Required]
        public string AddressName { get; set; }
        [Required]
        public string Receiver { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Postcode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Subdistrict { get; set; }
        [Required]
        public Nullable<int> SubdistrictId { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}