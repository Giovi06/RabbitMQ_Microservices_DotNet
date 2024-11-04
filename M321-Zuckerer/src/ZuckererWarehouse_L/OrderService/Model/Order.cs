namespace OrderService.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public Client Client { get; set; } = null!;
        public string ShipToAddress { get; set; }
        public DateTime PlacedAt { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public DateTime? ShippedAt { get; set; }
        public string? ShippedWith { get; set; }
        public string? ShipmentTrackingId { get; set; }
        public string? ShipmentTrackingUrl { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CanceledAt { get; set; }
    }
}
