using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Dao
{
    public class CheckOutDao
    {
        SWPContext _context;
        public CheckOutDao()
        {
            _context = new SWPContext();
        }
        public List<CartItem> getAddItem( int cusId , List<int> selectedItems)
        {
            try
            {
                var data = _context.CartItems
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Size)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Color)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.Category)
                    .Where(x=>x.CustomerId == cusId && selectedItems.Contains(x.DetailId)).ToList();
                if(data == null)
                {
                    return null;
                }
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool adddb(int cusId,string customerName, string phoneNumber, string address, List<int> detail)
        {
            try
            {
                var cart=_context.CartItems
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Size)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Color)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.Category)
                    .Where(s=>s.CustomerId.Equals(cusId)).ToList();
                var order= new Order();
                order.CustomerName= customerName;
                order.PhoneNumber= phoneNumber;
                order.UserId = cusId;
                order.OrderDate = DateTime.Now;
                order.PaymentMethod = 0;
                order.Status = 0;
                order.ShipAddress = address;

                _context.Orders.Add(order);
                _context.SaveChanges();

                var orderad = _context.Orders
                     .Where(s => s.UserId == cusId && s.OrderDate == order.OrderDate)
                     .OrderByDescending(s => s.OrderId) 
                     .LastOrDefault();
                foreach (var item in detail)
                {
                    foreach( var cartitem in cart)
                    {
                        if(item.Equals(cartitem.DetailId))
                        {
                            
                            var orrderdetail = new Orderdetail();
                            orrderdetail.OrderId = orderad.OrderId;
                            orrderdetail.DetailId = item;
                            orrderdetail.Quantity = cartitem.Quantity;
                            orrderdetail.CreatedDate = DateTime.Now;
                            orrderdetail.Price = cartitem.Detail.Product.Price;
                            _context.Orderdetails.Add(orrderdetail);
                         //   _context.SaveChanges();
                            var cartorderdetail = _context.CartItems.FirstOrDefault(s => s.DetailId == item);
                            _context.CartItems.Remove(cartorderdetail);
                            _context.SaveChanges();
                        }

                    }
                   
                    
                }
           

               
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public User getInformation(int cusId)
        {
            try
            {
                var data = _context.Users.Where(x => x.UserId == cusId).FirstOrDefault();
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
