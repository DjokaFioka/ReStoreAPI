using ReStoreAPI.Entities.OrderAggregate;

namespace ReStoreAPI.DTOs
{
    public class CreateOrderDto
    {
        public bool SaveAddress { get; set; }
        public ShippingAddress  ShippingAddress { get; set; }
    }
}
