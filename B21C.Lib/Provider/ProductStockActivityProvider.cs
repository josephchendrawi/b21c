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
    public class ProductStockActivityProvider
    {
        public IQueryable<PRODUCT_STOCK_ACTIVITY> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_STOCK_ACTIVITY
                         select d;

            return result;
        }

        public long Add(PRODUCT_STOCK_ACTIVITY vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCT_STOCK_ACTIVITY.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

    }
}
