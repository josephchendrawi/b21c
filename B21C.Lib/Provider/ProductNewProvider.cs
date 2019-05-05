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
    public class ProductNewProvider
    {
        public IQueryable<PRODUCT_TRENDING> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_TRENDING
                         select d;

            return result;
        }

        public IQueryable<long> GetAll_TrendingProductId()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_TRENDING
                         select d.ProductId;

            return result;
        }

        public IQueryable<long> GetAll_Valid_TrendingProductId()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_TRENDING
                         where d.ValidFrom <= DateTime.Now && d.ValidTo > DateTime.Now
                         select d.ProductId;

            return result;
        }

        public long Add(PRODUCT_TRENDING vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCT_TRENDING.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_TRENDING
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.PRODUCT_TRENDING.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

        public void Delete_ByProductId(long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_TRENDING
                             where d.ProductId == ProductId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.PRODUCT_TRENDING.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
