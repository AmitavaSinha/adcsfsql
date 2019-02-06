using System;
using System.Collections.Generic;

namespace Cart.DAL.Models
{
    public partial class OrderHeader
    {
        public OrderHeader()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderByUserName { get; set; }
        public string OrderByUserId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
