using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Dao
{
    public class ProductDao
    {
        SWPContext _context;
        public ProductDao()
        {
            _context = new SWPContext();
        }
        public Product getProductById(int id)
        {
            try
            {
                var pro = _context.Products
                    .Include(x => x.ProductDetails)                                             
                        .ThenInclude(x => x.Color)
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Include(x => x.ProductDetails)
                        .ThenInclude(x=>x.Size)
                    .Include(x => x.ProductImages)
                        .Where(x=>x.ProductId == id).FirstOrDefault();
                if(pro == null)
                {
                    return null;
                }
                return pro;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<Product> getProductByCategory(int categoryId)
        {
            try
            {
                var listPro = _context.Products.Include(x => x.ProductImages).Where(x => x.CategoryId == categoryId).ToList();
                return listPro;
            }
            catch (Exception)
            {

                return null;
            }
        }
        }
}
