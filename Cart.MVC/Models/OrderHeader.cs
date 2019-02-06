using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cart.MVC.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderByUserName { get; set; }
        public string OrderByUserId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
    }

    public enum PaymentMode
    {
        COD = 1
        
    }
}