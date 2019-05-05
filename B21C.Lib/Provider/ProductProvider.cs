using B21C.EF;
using B21C.Lib.Common.Constants;
using B21C.Lib.Common.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.Provider
{
    public class ProductProvider
    {
        public IQueryable<PRODUCT> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.PRODUCTs
                         where d.Status == STATUS_CODE.ACTIVE
                         select d;

            return result;
        }
        public IQueryable<PRODUCT_DTO> GetAll_DTO()
        {
            var DBContext = new b21cEntities();
            
            var NewProductIdList = (new ProductNewProvider()).GetAll_Valid_TrendingProductId().ToList();

            var result = from d in DBContext.PRODUCTs
                         where d.Status == STATUS_CODE.ACTIVE
                         select new PRODUCT_DTO()
                         {
                             Color = d.Color,
                             CreatedAt = d.CreatedAt,
                             CreatedBy = d.CreatedBy,
                             Description = d.Description,
                             LastUpdAt = d.LastUpdAt,
                             Id = d.Id,
                             LastUpdBy = d.LastUpdBy,
                             ModalPrice = d.ModalPrice,
                             Name = d.Name,
                             OutOfStock = d.Stock <= 0,
                             Price = d.Price,
                             ProductCode = d.ProductCode,
                             Sales = d.Sales,
                             SalesDiscount = d.SalesDiscount,
                             Size = d.Size,
                             Status = d.Status,
                             Stock = d.Stock,
                             OldStock = d.OldStock,
                             Weight = d.Weight,
                             Point = d.Point,
                             PublishAt = d.PublishAt,
                             Section = d.Section,

                             isNew = NewProductIdList.Contains(d.Id),

                             CategoryId = d.CategoryId,
                             PRODUCT_CATEGORY = d.PRODUCT_CATEGORY,
                         };

            return result;
        }

        public PRODUCT Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCTs.Include(m => m.PRODUCT_CATEGORY)
                             where d.Id == id
                             && d.Status == STATUS_CODE.ACTIVE
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    return v;
                }
            }

            return null;
        }

        public PRODUCT Get_DespiteStatus(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCTs
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

        public void Edit(PRODUCT vo, long ByUserId, string Action = "")
        {
            Action = Action == "" ? PRODUCT_MODIFICATION_ACTION.EDIT : Action;

            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCTs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    var BeforeEditedStock = Get(vo.Id).Stock;

                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.PRODUCTs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                    
                    RecordStockLog(Action, ByUserId, vo.Id, BeforeEditedStock ?? 0);
                }
            }
        }

        public long Add(PRODUCT vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.PRODUCTs.Add(vo);
                DBContext.SaveChanges();
                
                RecordStockLog(PRODUCT_MODIFICATION_ACTION.ADD, ByUserId);

                return vo.Id;
            }
        }

        public void Delete(long Id, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.PRODUCTs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.PRODUCTs.Remove(v);
                    DBContext.SaveChanges();

                    RecordStockLog(PRODUCT_MODIFICATION_ACTION.DELETE, ByUserId);
                }
            }
        }

        public bool CheckStockAvailability(List<ORDER_PRODUCT> Products)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from e in Products
                             join d in DBContext.PRODUCTs on e.ProductId equals d.Id
                             where e.Quantity > d.Stock
                             select e;

                if (result.Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void RecordStockLog(string Action, long ByUserId, long ProductId = -1, int BeforeEditStock = -1)
        {
            using (var DBContext = new b21cEntities())
            {
                var ProductStockLogProvider = new ProductStockLogProvider();

                var TodayLatestLog = ProductStockLogProvider.GetByDate(DateTime.Now).OrderByDescending(m => m.CreatedAt).FirstOrDefault();

                var TodayLatestLog_CanceledList = new List<string>();
                if (TodayLatestLog != null)
                {
                    if (!string.IsNullOrWhiteSpace(TodayLatestLog.Canceled))
                        TodayLatestLog_CanceledList = TodayLatestLog.Canceled.Split('|').ToList();
                }

                if (Action == PRODUCT_MODIFICATION_ACTION.CANCEL)
                {
                    var CanceledProduct = (from d in DBContext.PRODUCTs
                                           where d.Id == ProductId
                                           select d).FirstOrDefault();

                    if (CanceledProduct != null)
                    {
                        if (BeforeEditStock == 0)
                        {
                            TodayLatestLog_CanceledList.Add(CanceledProduct.Name);
                        }
                    }
                }
                else if(Action == PRODUCT_MODIFICATION_ACTION.EDIT)
                {
                    var EditedProduct = (from d in DBContext.PRODUCTs
                                         where d.Id == ProductId
                                         select d).FirstOrDefault();

                    if (EditedProduct != null)
                    {
                        //if stock increases
                        if (BeforeEditStock < EditedProduct.Stock) ///
                        {
                            TodayLatestLog_CanceledList.Remove(EditedProduct.Name);
                        }
                    }
                }
                else if (Action == PRODUCT_MODIFICATION_ACTION.BOOK)
                {
                    var BookedProduct = (from d in DBContext.PRODUCTs
                                         where d.Id == ProductId
                                         select d).FirstOrDefault();

                    if (BookedProduct != null)
                    {
                        TodayLatestLog_CanceledList.Remove(BookedProduct.Name);
                    }
                }


                ProductStockLogProvider.Add(new PRODUCT_STOCK_LOG()
                {
                    Available = string.Join("|", this.GetAll().Where(m => m.Stock > 0).Select(m => m.Name)),
                    SoldOut = string.Join("|", this.GetAll().Where(m => m.Stock == 0).Select(m => m.Name)),
                    Canceled = string.Join("|", TodayLatestLog_CanceledList),
                    Action = Action,
                }, ByUserId);
            }
        }

    }
}
