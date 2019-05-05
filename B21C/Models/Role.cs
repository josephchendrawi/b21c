using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Models
{
    public class RoleVM
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
    
    public class RoleAccessVM
    {
        public string AccessModule { get; set; }
        public bool Viewable { get; set; }
        public bool Addable { get; set; }
        public bool Editable { get; set; }
        public bool Deletable { get; set; }
    }
}