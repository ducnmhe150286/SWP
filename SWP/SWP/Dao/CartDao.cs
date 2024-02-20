using Microsoft.EntityFrameworkCore;
using SWP.Dto;
using SWP.Models;

namespace SWP.Dao
{
    public class CartDao
    {
        SWPContext _context;
        public CartDao()
        {
            _context = new SWPContext();
        }
        public CartItem addToCart(int productId , int quantity , int customerId )
        {
            try
            {
                
                var cart = _context.CartItems.Where(x=>x.ProductId == productId && x.CustomerId == customerId).FirstOrDefault();
                if(cart == null)
                {
                    var temp = new CartItem();
                    temp.CustomerId = customerId;
                    temp.ProductId = productId;
                    temp.CreateDate = DateTime.Now;
                    temp.Quantity = quantity;
                    _context.CartItems.Add(temp);
                }
                else
                {
                    cart.Quantity += quantity;
                }
                
                _context.SaveChanges();
                return cart;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<CartItem> GetAddItem(int customerId)
        {
            try
            {
                var data = _context.CartItems
                    .Include(x=>x.Product)
                        .ThenInclude(x=>x.ProductImages)
                    .Include(x => x.Product)
                        .ThenInclude(x => x.Category)
                    .ToList();
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public CartItem updateCartItem(int productId,int type,int cusId)
        {
            try
            {
                var data = _context.CartItems.Where(x => x.ProductId == productId && x.CustomerId == cusId).FirstOrDefault();
                if(type == 0 && data.Quantity >1)
                {
                    data.Quantity -= 1;
                }
                else if(type == 1){
                    data.Quantity += 1;
                }
                if(type == 2)
                {
                    _context.CartItems.Remove(data);
                }
                _context.SaveChanges();
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }
        

    }
}
