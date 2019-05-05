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
    public class LookupProvider
    {
        public IQueryable<LOOKUP> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.LOOKUPs
                         select d;

            return result;
        }
        public IQueryable<LOOKUP> GetAll(string Type)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.LOOKUPs
                         where d.Type == Type
                         select d;

            return result;
        }

        public LOOKUP Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.LOOKUPs
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
        public LOOKUP Get(string Name)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.LOOKUPs
                             where d.Name == Name
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public void Edit(LOOKUP vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.LOOKUPs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.LOOKUPs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(LOOKUP vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.LOOKUPs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.LOOKUPs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.LOOKUPs.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
