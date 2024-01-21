using Microsoft.AspNetCore.Mvc;

namespace SWP.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
