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
    public class ProductImageProvider
    {
        public List<PRODUCT_IMAGE> GetAllByProductId(long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_IMAGE
                             where d.ProductId == ProductId
                             orderby d.DisplayOrder ascending
                             select d;

                if (result.Count() > 0)
                    return result.ToList();
                else
                    return new List<PRODUCT_IMAGE>();
            }
        }

        public PRODUCT_IMAGE GetMainImageByProductId(long ProductId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_IMAGE
                             where d.ProductId == ProductId
                             orderby d.DisplayOrder ascending
                             select d;

                if (result.Count() > 0)
                    return result.First();
                else
                    return null;
            }
        }
        public PRODUCT_IMAGE Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_IMAGE
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

        public void Edit(PRODUCT_IMAGE vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_IMAGE
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.PRODUCT_IMAGE.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(PRODUCT_IMAGE vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCT_IMAGE.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCT_IMAGE
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.PRODUCT_IMAGE.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
