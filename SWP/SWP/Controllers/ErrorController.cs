using Microsoft.AspNetCore.Mvc;

namespace SWP.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
