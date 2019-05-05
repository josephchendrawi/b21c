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
    public class OrderProvider
    {
        public IQueryable<ORDER> GetAll()
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ORDERs
                         select d;

            return result;
        }

        public IQueryable<ORDER_DTO> GetAll_DTO(string Website = "All", string Type = "All")
        {
            var DBContext = new b21cEntities();

            var result = from d in DBContext.ORDERs
                         join e in DBContext.USERs on d.CreatedBy equals e.Id
                         where (Website != "All" ? e.Website == Website : true)
                         && (Type == "Admin" ?
                                e.Type != USER_TYPE.RESELLER && d.BookedBy != "Guest Buyer"
                        :
                        (
                            Type == "Store" ?
                                e.Type == USER_TYPE.RESELLER || d.BookedBy == "Guest Buyer"
                            :
                                true
                        ))
                         select new ORDER_DTO()
                         {
                             Address = d.Address,
                             ContactNo = d.ContactNo,
                             CreatedAt = d.CreatedAt,
                             CreatedBy = d.CreatedBy,
                             ExpirationDate = d.ExpirationDate,
                             FlgAdminLogo = d.FlgAdminLogo,
                             Id = d.Id,
                             LastUpdAt = d.LastUpdAt,
                             LastUpdBy = d.LastUpdBy,
                             OrderCode = d.OrderCode,
                             PaymentMethod = d.PaymentMethod,
                             Receiver = d.Receiver,
                             Sender = d.Sender,
                             ShippingFee = d.ShippingFee,
                             Status = d.Status,
                             TotalPrice = d.TotalPrice,
                             TotalWeight = d.TotalWeight,
                             TrackingNo = d.TrackingNo,
                             BookedBy = d.BookedBy,
                             Discount = d.Discount,
                             Shipping = d.Shipping,
                             ShippingCode = d.ShippingCode,
                             ShippingDate = d.ShippingDate,
                             PrepareShipmentDate = d.PrepareShipmentDate,
                             AdditionalDiscount = d.AdditionalDiscount,
                             PackingCode = d.PackingCode,

                             GrandTotal = (d.TotalPrice ?? 0) + (d.ShippingFee ?? 0) - (d.AdditionalDiscount ?? 0) + (d.PaymentCode ?? 0),
                             TotalPriceWithAdditionalDiscount = (d.TotalPrice ?? 0) - (d.AdditionalDiscount ?? 0) + (d.PaymentCode ?? 0),
                             PaymentExpired = (DateTime.Now > d.ExpirationDate) && (d.Status == ORDER_STATUS_CODE.PENDING_PAYMENT),
                             UserPaymentAmount = d.UserPaymentAmount,

                             BankAccountName = d.BankAccountName,
                             City = d.City,
                             Note = d.Note,
                             Province = d.Province,
                             Subdistrict = d.Subdistrict,
                             SubdistrictId = d.SubdistrictId,

                             TotalPartnerDiscount = d.TotalPartnerDiscount ?? 0,

                             CreatedBy_Name = e.Name == null ? "-" : e.Name,
                             CreatedBy_Type = e.Type == null ? "-" : e.Type,
                             CreatedBy_Website = e.Website,
                             CreatedBy_Role = e.ROLE == null ? "-" : e.ROLE.Name,

                             PackedCount = d.PackedCount ?? 0,

                             PaymentCode = d.PaymentCode,
                             ReceivedPaymentConfirmationAt = d.ReceivedPaymentConfirmationAt,

                             ORDER_PRODUCT = d.ORDER_PRODUCT
                         };

            return result;
        }

        public ORDER Get(long id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDERs
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

        public void Edit(ORDER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDERs
                             where d.Id == vo.Id
                             select d;

                if (result.Count() > 0)
                {
                    vo.LastUpdAt = DateTime.Now;
                    vo.LastUpdBy = ByUserId;
                    DBContext.ORDERs.Attach(vo);
                    DBContext.Entry(vo).State = EntityState.Modified;
                    DBContext.SaveChanges();
                }
            }
        }

        public long Add(ORDER vo, long ByUserId)
        {
            using (var DBContext = new b21cEntities())
            {
                var TodaysOrder = from d in DBContext.ORDERs
                                  where d.CreatedAt.Value.Year == DateTime.Now.Year && d.CreatedAt.Value.Month == DateTime.Now.Month && d.CreatedAt.Value.Day == DateTime.Now.Day
                                  select d;
                var TodaysOrderCount = TodaysOrder.Count();
                var TodaysOrderHighestPaymentCode = TodaysOrder.Max(m => m.PaymentCode) ?? 0;

                vo.CreatedAt = DateTime.Now;
                vo.CreatedBy = ByUserId;
                vo.LastUpdAt = DateTime.Now;
                vo.LastUpdBy = ByUserId;
                DBContext.ORDERs.Add(vo);
                DBContext.SaveChanges();

                vo.OrderCode = (vo.Id + 23648).ToString();

                vo.PaymentCode = TodaysOrderCount > 0 ? TodaysOrderHighestPaymentCode + 1 : 1;
                if (vo.UserPaymentAmount != null && vo.UserPaymentAmount > 0)
                    vo.UserPaymentAmount += vo.PaymentCode;

                DBContext.SaveChanges();

                return vo.Id;
            }
        }

        public void Delete(long Id)
        {
            using (var DBContext = new b21cEntities())
            {
                var result = from d in DBContext.ORDERs
                             where d.Id == Id
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    DBContext.ORDERs.Remove(v);
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
