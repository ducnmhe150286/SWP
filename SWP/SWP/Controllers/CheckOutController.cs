using Microsoft.AspNetCore.Mvc;

namespace SWP.Controllers
{
    public class CheckOutController : Controller
    {
        [HttpPost]
        public IActionResult Index(List<int> selectedItems)
        {
            //get detail
            // formdata[]
            return View();
        }
    }
}
