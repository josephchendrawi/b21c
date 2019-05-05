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
    public class ProductCategoryProvider
    {
        public IQueryable<PRODUCT_CATEGORY> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_CATEGORY
                         select d;

            return result;
        }

        public PRODUCT_CATEGORY Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_CATEGORY
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

        public void Edit(PRODUCT_CATEGORY vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_CATEGORY
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.PRODUCT_CATEGORY.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(PRODUCT_CATEGORY vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCT_CATEGORY.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_CATEGORY
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.PRODUCT_CATEGORY.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
