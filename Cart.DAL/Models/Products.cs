using System;
using System.Collections.Generic;

namespace Cart.DAL.Models
{
    public partial class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }
        public int Id { get; set; }
        public string ProductCategory { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
