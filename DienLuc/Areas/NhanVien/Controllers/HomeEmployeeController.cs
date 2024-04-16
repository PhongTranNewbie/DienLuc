using Microsoft.AspNetCore.Mvc;

namespace DienLuc.Areas.NhanVien.Controllers
{
    public class HomeEmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
