using System;

namespace ShopKart.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quentity { get; set; }
        public int Price { get; set; }
    }
}
