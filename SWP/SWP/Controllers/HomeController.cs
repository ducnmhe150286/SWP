using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Models;
using System.Diagnostics;

namespace SWP.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;
        public UsersDao usersDao;

        public HomeController(ILogger<HomeController> logger)
        {
            usersDao = new UsersDao();
            _logger = logger;

        }


        public ActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
        
    }
}