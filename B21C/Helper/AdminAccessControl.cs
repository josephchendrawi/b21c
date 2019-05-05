using B21C.Enums;
using B21C.Lib.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B21C.Helper.AccessControl
{
    public class AccessControl
    {
        public static bool IsAccessable(long AdminId, AccessModule AccessModule, string Access = "VIEW")
        {
            var Admin = new AdminProvider().Get(AdminId);
            var RoleAccess = new RoleAccessProvider().Get(Admin.RoleId ?? 0, AccessModule.ToString());

            if (RoleAccess != null)
            {
                switch (Access)
                {
                    case "VIEW":
                        return RoleAccess.Viewable ?? false;
                    case "ADD":
                        return RoleAccess.Addable ?? false;
                    case "EDIT":
                        return RoleAccess.Editable ?? false;
                    case "DELETE":
                        return RoleAccess.Deletable ?? false;
                }
            }

            return false;
        }
    }
}