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
    }
}
