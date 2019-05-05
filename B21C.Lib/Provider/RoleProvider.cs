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
    public class RoleProvider
    {
        public IQueryable<ROLE> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ROLEs
                         select d;

            return result;
        }

        public ROLE Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ROLEs
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

        public void Edit(ROLE vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ROLEs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.ROLEs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(ROLE vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.ROLEs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ROLEs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.ROLEs.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
