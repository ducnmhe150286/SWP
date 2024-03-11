using SWP.Models;

namespace SWP.Dao
{
   
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string ShipAddress { get; set; }
        public DateTime? OrderDate { get; set; }
        public string PhoneNumber { get; set; }
        public byte? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderProductDetailViewModel> OrderProductDetails { get; set; }

    }
}
