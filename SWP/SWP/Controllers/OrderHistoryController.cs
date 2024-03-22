using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Models;

namespace SWP.Controllers
{
    public class OrderHistoryController : Controller
    {
        public UsersDao userDao;
        public OrderHistoryDao historyDao;
        public OrderHistoryController()
        {
            userDao = new UsersDao();
            historyDao = new OrderHistoryDao();
        }
        public IActionResult Index()
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");

            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var getOrder = historyDao.getAllOrder(cusId.UserId);
                if(getOrder == null)
                {
                    ViewData["message_none"] = "Không có đơn hàng nào đã đặt.";
                    return View();
                }
                ViewData["listItem"] = getOrder;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }

        public List<Orderdetail> GetOrderDetail(int orderId)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");

            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var orderDetail = historyDao.getOrderDetailByOrderId(orderId);
                
                return orderDetail;
            }
            return null;
        }
    }
}
