namespace SWP.Dao
{
    public class OrderProductDetailViewModel
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public List<string> ProductImages { get; set; }
        public int OrderId { get; set; } // Thêm thuộc tính orderId
    }
}
