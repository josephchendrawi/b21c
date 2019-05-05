using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class AdminVM
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        public string RoleName { get; set; }
    }

    public class AdminAddVM : AdminVM
    {
        [Required]
        [Display(Name = "Role")]
        public long? RoleId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password length is incorrect (at least 6 characters)")]
        public string Password { get; set; }
    }

    public class AdminEditVM : AdminVM
    {
        [Required]
        [Display(Name = "Role")]
        public long? RoleId { get; set; }
        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password length is incorrect (at least 6 characters)")]
        public string Password { get; set; }
        public bool isPasswordChange { get; set; }
    }

    public class BuyerVM
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email Address not in the correct format")]
        public string Username { get; set; }
        [Display(Name = "Phone No.")]
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Balance { get; set; }
        public int Point { get; set; }
        public string Type { get; set; }
        [Display(Name = "Status")]
        public string UserStatus { get; set; }
        public DateTime? RegisteredAt { get; set; }
    }

    public class BuyerEditVM
    {
        public long Id { get; set; }
        public string Username { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Phone No.")]
        public string Phone { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime? BirthDate { get; set; }
        public int Point { get; set; }

        public bool isUserStatusChange { get; set; }
        [Display(Name = "Status")]
        public long UserStatusId { get; set; }
    }

    public class BuyerChangeBalanceVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int Balance { get; set; }
        public string Remarks { get; set; }
    }

}