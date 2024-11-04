namespace OrderService.Model
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Order Order { get; set; }
        public int ItemPosition { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleDescription { get; set; }
        public int Quantity { get; set; }
    }
}
