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
    public class AddressBookProvider
    {
        public IQueryable<ADDRESS_BOOK> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ADDRESS_BOOK
                         select d;

            return result;
        }
        public IQueryable<ADDRESS_BOOK> GetAll(long UserId)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ADDRESS_BOOK
                         where d.UserId == UserId
                         select d;

            return result;
        }

        public ADDRESS_BOOK Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ADDRESS_BOOK
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

        public void Edit(ADDRESS_BOOK vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ADDRESS_BOOK
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.ADDRESS_BOOK.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(ADDRESS_BOOK vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.ADDRESS_BOOK.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ADDRESS_BOOK
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.ADDRESS_BOOK.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
