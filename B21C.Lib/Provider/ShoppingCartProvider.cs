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
    public class ShoppingCartProvider
    {
        public IQueryable<SHOPPING_CART> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.SHOPPING_CART
                         select d;

            return result;
        }

        public SHOPPING_CART Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
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

        public SHOPPING_CART Get(long UserId, long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.UserId == UserId && d.ProductId == ProductId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public SHOPPING_CART Get(string GuestId, long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.GuestID == GuestId && d.ProductId == ProductId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public void Edit(SHOPPING_CART vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.SHOPPING_CART.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(SHOPPING_CART vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.SHOPPING_CART.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.SHOPPING_CART.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

        public void DeleteByUserId(long UserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.UserId == UserId
                             select d;

                if (result.Count() > 0)
                {
                    DBContext.SHOPPING_CART.RemoveRange(result);
                    DBContext.SaveChanges();
                }
            }
        }

        public void DeleteByGuestID(string GuestID)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SHOPPING_CART
                             where d.GuestID == GuestID
                             select d;

                if (result.Count() > 0)
                {
                    DBContext.SHOPPING_CART.RemoveRange(result);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
