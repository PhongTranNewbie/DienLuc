using Microsoft.AspNetCore.Mvc;

namespace Dienluc.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
