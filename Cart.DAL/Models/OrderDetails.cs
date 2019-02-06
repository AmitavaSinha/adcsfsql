using System;
using System.Collections.Generic;

namespace Cart.DAL.Models
{
    public partial class OrderDetails
    {
        public int Id { get; set; }
        public int? OrderHeaderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }

        public OrderHeader OrderHeader { get; set; }
        public Products Product { get; set; }
    }
}
