using B21C.EF;
using B21C.Lib.Common.Constants;
using B21C.Lib.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Provider
{
    public class AdminProvider
    {
        public static bool IsAuthenticated(long UserID, string Username, string Website)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == UserID
                             && d.Username == Username
                             && d.Status == STATUS_CODE.ACTIVE
                             && d.Type == USER_TYPE.ADMIN
                             && d.Website == Website
                             select d;

                if (result.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public USER Login(string username, string password, string website)
        {
            var encrypted = Helper.CalculateMD5Hash(password);

            using (var DBContext = new b21cEntities())
            {
                var ADMIN = from d in DBContext.USERs
                            where d.Username.ToLower() == username.ToLower()
                            && d.Type == USER_TYPE.ADMIN
                            && d.Website == website
                            orderby d.Id descending
                            select d;
                if (ADMIN.Count() > 0)
                {
                    var Admin = ADMIN.First();

                    if (Admin.Status != STATUS_CODE.ACTIVE)
                    {
                        throw new Exception("This Admin is inactive.");
                    }

                    if (Admin.Password == encrypted)
                    {
                        return Admin;
                    }
                }
            }

            return null;
        }

        public List<USER> GetAllAdminByWebsite(string Website)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs.Include(m => m.ROLE)
                             where d.Website == Website
                             && d.Type == USER_TYPE.ADMIN
                             && d.Status == STATUS_CODE.ACTIVE
                             select d;
                
                return result.ToList();
            }
        }

        public List<USER> GetAllUserByWebsite(string Website)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs.Include(m => m.ROLE)
                             where d.Website == Website
                             && d.Status == STATUS_CODE.ACTIVE
                             select d;

                return result.ToList();
            }
        }


        public USER Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == id
                             && d.Type == USER_TYPE.ADMIN
                             && d.Status == STATUS_CODE.ACTIVE
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }
        public USER Get(string username, string website)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Username.ToLower() == username.ToLower()
                             && d.Website == website
                             && d.Type == USER_TYPE.ADMIN
                             && d.Status == STATUS_CODE.ACTIVE
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public void Edit(USER vo, bool ChangePassword, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    if (ChangePassword)
                        vo.Password = Helper.CalculateMD5Hash(vo.Password);

                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(USER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.Password = Helper.CalculateMD5Hash(vo.Password);

                vo.Type = USER_TYPE.ADMIN;
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.USERs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

    }
}
