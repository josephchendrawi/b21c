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
    public class ProductStockLogProvider
    {
        public IQueryable<PRODUCT_STOCK_LOG> GetByDate(DateTime Date)
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCT_STOCK_LOG
                         where d.CreatedAt != null
                         && d.CreatedAt.Value.Year == Date.Year
                         && d.CreatedAt.Value.Month == Date.Month
                         && d.CreatedAt.Value.Day == Date.Day
                         select d;

            return result;
        }

        public void Add(PRODUCT_STOCK_LOG vo, long ByUserId)
        {
            var Today = DateTime.Now;
            using (var DBContext = new b21cEntities())
            {
                var Logs = from d in DBContext.PRODUCT_STOCK_LOG
                           where d.CreatedAt != null
                           && d.CreatedAt.Value.Year == Today.Year
                           && d.CreatedAt.Value.Month == Today.Month
                           && d.CreatedAt.Value.Day == Today.Day
                           orderby d.Id descending
                           select d;

                if (Logs.Count() > 0)
                {
                    Logs.First().LastUpdAt = DateTime.Now;
                    Logs.First().LastUpdBy = ByUserId;

                    Logs.First().Action = vo.Action;
                    Logs.First().Available = vo.Available;
                    Logs.First().SoldOut = vo.SoldOut;
                    Logs.First().Canceled = vo.Canceled;

                    DBContext.SaveChanges();
                }
                else
                {
                    vo.CreatedAt = DateTime.Now;
                    vo.CreatedBy = ByUserId;
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.PRODUCT_STOCK_LOG.Add(vo);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
