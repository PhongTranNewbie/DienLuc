using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;

namespace DienLuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/DoanhThu")]
    public class DoanhThuController : Controller
    {
        private readonly DienLucContext _context;
        public DoanhThuController(DienLucContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("")]
        public IActionResult Index(DateTime? startdate, DateTime? endDate)
        {
            decimal sotien = 0;
            var dt = _context.HoaDons
                .Where(x => x.NgayTao.Date >= startdate && x.NgayTao.Date <= endDate && x.TrangThai == "Đã thanh toán")
                .ToList();
            for(var i = 0;i<dt.Count;i++)
            {
                sotien += dt[i].SoTienDaThanhToan;
            }

            // Chuẩn bị dữ liệu cho biểu đồ
            var doanhThuData = new
            {
                labels = dt.Where(x => x.MaHd != null).Select(x => x.MaHd),
                data = dt.Where(x => x.NgayThanhToan != null).Select(x => x.SoTienDaThanhToan)
            };
            ViewBag.DoanhThuData = doanhThuData;


            ViewBag.DoanhThu = sotien;
            ViewBag.HD = dt.Count;
            return PartialView("Index");
        }
        
    }
}
