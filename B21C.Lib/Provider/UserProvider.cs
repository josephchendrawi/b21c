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
    public class UserProvider
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
                            && d.Type == USER_TYPE.RESELLER
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

        public List<USER> GetAllBuyerByWebsite(string Website)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs.Include(m => m.USER_STATUS)
                             where d.Website == Website
                             && d.Type == USER_TYPE.RESELLER
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
                             && d.Status == STATUS_CODE.ACTIVE
                             && d.Type == USER_TYPE.RESELLER
                             select d;

                if (result.Count() > 0)
                {
                    result = result.Include(m => m.USER_STATUS);

                    var v = result.First();
                    v.ADDRESS_BOOK = v.ADDRESS_BOOK.ToList();
                    v.SHOPPING_CART = v.SHOPPING_CART.ToList();
                    v.USER_BALANCE_TRX = v.USER_BALANCE_TRX.ToList();

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
                             && d.Status == STATUS_CODE.ACTIVE
                             && d.Type == USER_TYPE.RESELLER
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public long Add(USER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.Password = Helper.CalculateMD5Hash(vo.Password);

                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.USERs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Edit(USER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public void EditPassword(long UserId, string NewPassword, long ByUserId)
        {
            NewPassword = Helper.CalculateMD5Hash(NewPassword);

            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == UserId
                             select d;

                if (result.Count() > 0)
                {
                    var vo = result.First();

                    vo.Password = NewPassword;
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public void AddBalance(long ToUserId, decimal AddAmount, string Remarks, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == ToUserId
                             select d;

                if (result.Count() > 0)
                {
                    var vo = result.First();

                    var BalanceBefore = vo.Balance;

                    vo.Balance += AddAmount;
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();

                    new UserBalanceTrxProvider().Add(new USER_BALANCE_TRX()
                    {
                        Amount = AddAmount,
                        Remarks = "UPDATE : From " + (int)BalanceBefore + " To " + (int)(BalanceBefore + AddAmount) + System.Environment.NewLine + Remarks,
                        UserId = ToUserId,
                    }, ByUserId);
                }
            }
        }

        public void UpdateBalance(long ToUserId, decimal ToAmount, string Remarks, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == ToUserId
                             select d;

                if (result.Count() > 0)
                {
                    var vo = result.First();

                    var BalanceBefore = vo.Balance;

                    vo.Balance = ToAmount;
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();

                    new UserBalanceTrxProvider().Add(new USER_BALANCE_TRX()
                    {
                        Amount = ToAmount - BalanceBefore,
                        Remarks = "UPDATE : From " + (int)BalanceBefore + " To " + (int)ToAmount + System.Environment.NewLine + Remarks,
                        UserId = ToUserId,
                    }, ByUserId);
                }
            }
        }

        public void AddPoint(long ToUserId, int AddPointAmount, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == ToUserId
                             select d;

                if (result.Count() > 0)
                {
                    var vo = result.First();

                    var BalanceBefore = vo.Balance;

                    vo.Point += AddPointAmount;
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();

                    UpdateStatusBasedOnPoint(vo.Id);
                }
            }
        }

        public void UpdateStatusBasedOnPoint(long UserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USERs
                             where d.Id == UserId
                             select d;
                
                if (result.Count() > 0)
                {
                    var vo = result.First();

                    long StatusId = 0;
                    var StatusList = new UserStatusProvider().GetAll();
                    foreach (var v in StatusList.OrderBy(m => m.PointNeeded))
                    {
                        if (v.PointNeeded <= vo.Point)
                            StatusId = v.Id;
                        else
                            break;
                    }

                    vo.UserStatusId = StatusId;
                    vo.LastUpdAt = DateTime.Now;

                    DBContext.SaveChanges();
                }
            }
        }

    }
}
