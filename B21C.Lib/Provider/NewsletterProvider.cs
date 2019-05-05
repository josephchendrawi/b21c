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
    public class NewsletterProvider
    {
        public IQueryable<EMAIL> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.EMAILs
                         select d;

            return result;
        }

        public EMAIL Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAILs
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

        public void Edit(EMAIL vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAILs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.EMAILs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(EMAIL vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.EMAILs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAILs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.EMAILs.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
