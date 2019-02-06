using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cart.MVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductCategory { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
    }

    public class UserEntity
    {
        public string userId { get; set; }
        public int productId { get; set; }

    }

    public class ConfirmOrder {
        public string userId { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
    }
}