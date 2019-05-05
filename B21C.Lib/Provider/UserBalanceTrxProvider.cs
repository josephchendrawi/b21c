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
    public class UserBalanceTrxProvider
    {
        public IQueryable<USER_BALANCE_TRX> GetAll(string Website)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.USER_BALANCE_TRX.Include(m => m.USER)
                         where d.USER.Website == Website
                         select d;

            return result;
        }

        public IQueryable<USER_BALANCE_TRX> GetAll(long UserId)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.USER_BALANCE_TRX.Include(m => m.USER)
                         where d.UserId == UserId
                         select d;

            return result;
        }

        public USER_BALANCE_TRX Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TRX
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

        public void Edit(USER_BALANCE_TRX vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TRX
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.USER_BALANCE_TRX.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(USER_BALANCE_TRX vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.USER_BALANCE_TRX.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.USER_BALANCE_TRX
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.USER_BALANCE_TRX.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
