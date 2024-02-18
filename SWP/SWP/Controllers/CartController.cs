using Microsoft.AspNetCore.Mvc;

namespace SWP.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
