using B21C.EF;
using B21C.Lib.Common.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Provider
{
    public class RoleAccessProvider
    {
        public ROLE_ACCESS Get(long RoleId, string AccessModule)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ROLE_ACCESS
                         where d.RoleId == RoleId
                         && d.AccessModule == AccessModule
                         select d;

            if (result.Count() > 0)
            {
                return result.First();
            }
            else
            {
                return null;
            }
        }

        public void SetAccess(ROLE_ACCESS vo, long ByUserId)
        {
            using (var context = new b21cEntities())
            {
                var RoleAccesses = from d in context.ROLE_ACCESS
                                   where d.RoleId == vo.RoleId
                                   && d.AccessModule == vo.AccessModule
                                   select d;

                if (RoleAccesses.Count() > 0)
                {
                    var RoleAccess = RoleAccesses.First();

                    RoleAccess.Viewable = vo.Viewable;
                    RoleAccess.Addable = vo.Addable;
                    RoleAccess.Editable = vo.Editable;
                    RoleAccess.Deletable = vo.Deletable;

                    RoleAccess.LastUpdAt = DateTime.Now;
                    RoleAccess.LastUpdBy = ByUserId;

                    context.SaveChanges();
                }
                else
                {
                    //create access
                    ROLE_ACCESS RoleAccess = new ROLE_ACCESS();
                    RoleAccess.RoleId = vo.RoleId;
                    RoleAccess.AccessModule = vo.AccessModule;
                    RoleAccess.Viewable = vo.Viewable;
                    RoleAccess.Addable = vo.Addable;
                    RoleAccess.Editable = vo.Editable;
                    RoleAccess.Deletable = vo.Deletable;

                    RoleAccess.CreatedAt = DateTime.Now;
                    RoleAccess.CreatedBy = ByUserId;
                    RoleAccess.LastUpdAt = DateTime.Now;
                    RoleAccess.LastUpdBy = ByUserId;

                    context.ROLE_ACCESS.Add(RoleAccess);
                    context.SaveChanges();
                }
            }
        }

    }
}
