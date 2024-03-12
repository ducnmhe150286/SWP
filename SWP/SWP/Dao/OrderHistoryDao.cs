using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Dao
{
    public class OrderHistoryDao
    {
        SWPContext _context;
        public OrderHistoryDao()
        {
            _context = new SWPContext();
        }
        public List<Order> getAllOrder(int cusId)
        {
            try
            {
                var data = _context.Orders.Where(x => x.UserId == cusId && (x.Status == 5 || x.Status == 6)).ToList();
                if(data.Count > 0)
                {
                    return data;
                }
                
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public List<Orderdetail> getOrderDetailByOrderId(int orderId)
        {
            try
            {
                var data = _context.Orderdetails
                    .Include(x => x.Order)
                    .Include(x => x.Detail)
                        .ThenInclude(x=>x.Product)
                        .ThenInclude(x=>x.ProductImages)
                    .Where(x=> x.OrderId== orderId).ToList();
                if(data == null)
                {
                    return null;
                }
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
