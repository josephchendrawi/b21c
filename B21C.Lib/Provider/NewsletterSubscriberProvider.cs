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
    public class NewsletterSubscriberProvider
    {
        public IQueryable<EMAIL_SUBSCRIBER> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.EMAIL_SUBSCRIBER
                         select d;

            return result;
        }

        public EMAIL_SUBSCRIBER Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAIL_SUBSCRIBER
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

        public void Edit(EMAIL_SUBSCRIBER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAIL_SUBSCRIBER
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.EMAIL_SUBSCRIBER.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(EMAIL_SUBSCRIBER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.EMAIL_SUBSCRIBER.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.EMAIL_SUBSCRIBER
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.EMAIL_SUBSCRIBER.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
