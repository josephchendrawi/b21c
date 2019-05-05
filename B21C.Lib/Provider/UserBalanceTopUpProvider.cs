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
    public class UserBalanceTopUpProvider
    {
        public IQueryable<USER_BALANCE_TOPUP> GetAll(string Website)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.USER_BALANCE_TOPUP.Include(m => m.USER)
                         where d.USER.Website == Website
                         select d;

            return result;
        }

        public IQueryable<USER_BALANCE_TOPUP> GetAll(long UserId)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.USER_BALANCE_TOPUP.Include(m => m.USER)
                         where d.UserId == UserId
                         select d;

            return result;
        }

        public USER_BALANCE_TOPUP Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TOPUP
                             where d.Id == id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public void Edit(USER_BALANCE_TOPUP vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TOPUP
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USER_BALANCE_TOPUP.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(USER_BALANCE_TOPUP vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.USER_BALANCE_TOPUP.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TOPUP
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.USER_BALANCE_TOPUP.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

        public void ChangeStatus(long Id, string Status, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TOPUP
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var vo = result.First();

                    vo.Status = Status;

                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USER_BALANCE_TOPUP.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
