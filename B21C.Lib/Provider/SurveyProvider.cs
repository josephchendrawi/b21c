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
    public class SurveyProvider
    {
        public IQueryable<SURVEY> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.SURVEYs
                         select d;

            return result;
        }

        public List<SURVEY> GetUncompletedSurveyByUser(long UserId)
        {
            var DBContext = new b21cEntities();

            var CompletedSurveyIds = from d in DBContext.USER_SURVEY
                                     where d.UserId == UserId
                                     select d.SurveyId;

            var UncompletedSurvey = from d in DBContext.SURVEYs
                                    where d.Status == STATUS_CODE.ACTIVE
                                    where !CompletedSurveyIds.Contains(d.Id)
                                    select d;

            return UncompletedSurvey.ToList();
        }

        public SURVEY Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SURVEYs
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

        public void Edit(SURVEY vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SURVEYs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.SURVEYs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(SURVEY vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.SURVEYs.Add(vo);
                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.SURVEYs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.SURVEYs.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
