using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cart.MVC.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int? OrderHeaderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }

        public OrderHeader OrderHeader { get; set; }
        public Product Product { get; set; }
    }
}