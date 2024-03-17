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
            if(productId == 0)
            {
                productId = (int)TempData["productId"];
            }
            var pro = proDetail.getProductById(productId);
            var listProSimilar = proDetail.getProductByCategory(pro);
            ViewData["listPro"] = listProSimilar;
            ViewData["error_quantity"] = TempData["error_quantity"];
            return View(pro);
        }
    }
}
