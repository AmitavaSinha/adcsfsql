using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cart.MVC.Models
{
    public class CartProducts
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductCategory { get; set; }        
        public decimal? ProductPrice { get; set; }
        public int? Quantity { get; set; }

        public int? OrderHeaderId { get; set; }

        public int OrderDetailId { get; set; }
    }
}