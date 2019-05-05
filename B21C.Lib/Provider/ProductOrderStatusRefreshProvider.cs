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
    public class ProductOrderStatusResetProvider
    {
        public IQueryable<PRODUCT_ORDER_STATUS_RESET> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_ORDER_STATUS_RESET
                         select d;

            return result;
        }

        public PRODUCT_ORDER_STATUS_RESET GetLatest(long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = (from d in DBContext.PRODUCT_ORDER_STATUS_RESET
                              where d.ProductId == ProductId
                              orderby d.Id descending
                              select d).FirstOrDefault();

                return result;
            }
        }

        public long Add(PRODUCT_ORDER_STATUS_RESET vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCT_ORDER_STATUS_RESET.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

    }
}
