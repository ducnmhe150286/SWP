using Microsoft.EntityFrameworkCore;
using SWP.Dto;
using SWP.Models;
using System.Drawing;

namespace SWP.Dao
{
    public class CartDao
    {
        SWPContext _context;
        public CartDao()
        {
            _context = new SWPContext();
        }
        public CartItem addToCart(int productId , int quantity  , int size , int color, int customerId )
        {
            try
            {
                var detail = _context.ProductDetails.FirstOrDefault(x => x.ProductId == productId && x.SizeId == size && x.ColorId == color);
                
                var cart = _context.CartItems.Where(x=>x.DetailId == detail.DetailId && x.CustomerId == customerId).FirstOrDefault();
                if(cart == null)
                {
                    var temp = new CartItem();
                    temp.CustomerId = customerId;
                    temp.DetailId = detail.DetailId;
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

        public int? CheckQuantity(int productId, int quantity, int size, int color)
        {
            try
            {
                var detail = _context.ProductDetails.FirstOrDefault(x => x.ProductId == productId && x.SizeId == size && x.ColorId == color);

                if(detail.Quantity < quantity)
                {
                    var check_quantity = detail.Quantity;
                    return check_quantity;
                }
                
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public List<CartItem> GetAddItem(int customerId)
        {
            try
            {
                var data = _context.CartItems
                    .Include(x=>x.Detail)
                        .ThenInclude(x=>x.Product)
                        .ThenInclude(x=>x.ProductImages)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Size)
                    .Include(x => x.Detail)
                        .ThenInclude(x => x.Color)
                    .Include(x=>x.Detail)
                        .ThenInclude(x=>x.Product)
                        .ThenInclude(x=>x.Category)
                    .Where(x=>x.CustomerId == customerId)
                    .ToList();
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

        public CartItem updateCartItem(int detailId , int type,int cusId)
        {
            try
            {
                
                var data = _context.CartItems.Where(x => x.DetailId == detailId && x.CustomerId == cusId).FirstOrDefault();
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
