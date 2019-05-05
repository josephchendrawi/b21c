using B21C.EF;
using B21C.Lib.Common.Constants;
using B21C.Lib.Common.DTO;
using B21C.Lib.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B21C.Lib.BLL
{
    public class ProductBLL
    {
        private ProductProvider ProductProvider = new ProductProvider();
        private UserProvider UserProvider = new UserProvider();
        private UserStatusProvider UserStatusProvider = new UserStatusProvider();

        public int GetUserDiscount(long UserId)
        {
            try
            {
                var User = UserProvider.Get(UserId);
                if (User.Type == USER_TYPE.RESELLER)
                {
                    var UserStatus = UserStatusProvider.Get(User.UserStatusId ?? 0);

                    if (UserStatus != null)
                    {
                        return (int)UserStatus.Discount;
                    }
                }
            }
            catch { }

            return 0;
        }

        public IEnumerable<PRODUCT_DTO> GetAll_DTO(long UserId = 0)
        {
            var ProductDiscount = 0;
            if (UserId != 0)
            {
                ProductDiscount = GetUserDiscount(UserId);
            }

            var result = ProductProvider.GetAll_DTO().Where(m => m.PublishAt <= DateTime.Now).ToList();

            foreach (var v in result) ///
            {
                //if Sales not additional discount anymore.
                if (v.Sales == null || v.Sales == false)
                {
                    v.Price = v.Price - ProductDiscount;
                }
            }

            return result;
        }

        public PRODUCT Get(long id, long UserId = 0)
        {
            var result = ProductProvider.Get(id);

            var ProductDiscount = 0;
            if (UserId != 0)
            {
                ProductDiscount = GetUserDiscount(UserId);
            }

            //if Sales not additional discount anymore.
            if (result.Sales == null || result.Sales == false)
            {
                result.Price = result.Price - ProductDiscount;
            }

            return result;
        }
    }
}
