using Microsoft.AspNetCore.Mvc;
using SWP.Dao;

namespace SWP.Controllers
{
    public class ProductDetailController : Controller
    {
        public ProductDao proDetail;
        public ProductDetailController()
        {
            this.proDetail = new ProductDao();
        }
        public IActionResult Index(int productId)
        {
            var pro = proDetail.getProductById(productId);
            var listProSimilar = proDetail.getProductByCategory(pro);
            ViewData["listPro"] = listProSimilar; 
            return View(pro);
        }
    }
}
