using System.ComponentModel.DataAnnotations;

namespace ReStoreAPI.Entities.OrderAggregate
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        [Required]
        public ShippingAddress ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItem> OrderItems { get; set; }
        public long Subtotal { get; set; }
        public long DeliveryFee { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public long GetTotal()
        {
            return Subtotal + DeliveryFee;
        }
    }
}
