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
    public class OrderProductProvider
    {
        public ORDER_PRODUCT Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDER_PRODUCT
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

        public List<ORDER_PRODUCT> GetAllByOrderId(long OrderId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDER_PRODUCT
                             where d.OrderId == OrderId
                             select d;

                if (result.Count() > 0)
                    return result.ToList();
                else
                    return new List<ORDER_PRODUCT>();
            }
        }
        public IQueryable<ORDER_PRODUCT> GetAllByProductId(long ProductId)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ORDER_PRODUCT
                         where d.ProductId == ProductId
                         select d;

            return result;
        }
        public IQueryable<ORDER_PRODUCT> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ORDER_PRODUCT
                         select d;

            return result;
        }

        public long Add(ORDER_PRODUCT vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.ORDER_PRODUCT.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Edit(ORDER_PRODUCT vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDER_PRODUCT
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.ORDER_PRODUCT.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDER_PRODUCT
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.ORDER_PRODUCT.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
