using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartService
{
    public class UserEntity
    {
        public string userId { get; set; }
        public int ProductId { get; set; }
    }

    public class CartOrder
    {
        public string userId { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
    }

}
