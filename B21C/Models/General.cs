using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B21C.Models
{
    public class AjaxResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ReturnURL { get; set; }
        public object Result { get; set; }
    }

    public class NotificationAjaxResult
    {
        public List<object> NotificationList { get; set; }
        public int TotalCount { get; set; }
    }
}