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
    }
}
