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


        [HttpGet("LietKe")]
        public IActionResult LietKe()
        {
            return View();
        }


        [HttpPost("LietKe")]
        public IActionResult LietKe(DateTime? startdate, DateTime? endDate)
        {
            if (startdate == null || endDate == null)
            {
                return BadRequest("Ngày bắt đầu và ngày kết thúc không hợp lệ.");
            }

            var hoadons = _context.HoaDons
                .Where(x => x.NgayTao.Date >= startdate && x.NgayTao.Date <= endDate)
                .ToList();

            var daThanhToanThuanCount = hoadons.Count(x => x.TrangThai == "Đã thanh toán" && (x.SoTienCanThanhToan - x.SoTienDaThanhToan) == 0);
            var chuaThanhToanCount = hoadons.Count(x => x.TrangThai == "Chưa thanh toán");
            var conNoCount = hoadons.Count(x => x.TrangThai == "Đã thanh toán" && (x.SoTienCanThanhToan - x.SoTienDaThanhToan) != 0);

            ViewBag.DaThanhToanThuanCount = daThanhToanThuanCount;
            ViewBag.ChuaThanhToanCount = chuaThanhToanCount;
            ViewBag.ConNoCount = conNoCount;
            ViewBag.HoaDons = hoadons;

            // Chuẩn bị dữ liệu cho biểu đồ
            var chartData = new
            {
                labels = new[] { "Hóa đơn đã thanh toán", "Hóa đơn chưa thanh toán" },
                datasets = new[]
                {
            new { label = "Còn nợ", data = new[] { conNoCount, 0 }, backgroundColor = "rgba(255, 206, 86, 0.2)", borderColor = "rgba(255, 206, 86, 1)" },
            new { label = "Đã thanh toán", data = new[] { daThanhToanThuanCount, 0 }, backgroundColor = "rgba(75, 192, 192, 0.2)", borderColor = "rgba(75, 192, 192, 1)" },
            new { label = "Chưa thanh toán", data = new[] { 0, chuaThanhToanCount }, backgroundColor = "rgba(255, 99, 132, 0.2)", borderColor = "rgba(255, 99, 132, 1)" }
        }
            };

            ViewBag.ChartData = chartData;

            return PartialView("LietKe", hoadons);
        }









        [HttpGet("DoanhThuThang")]
        public IActionResult DoanhThuThang()
        {
            return View();
        }


        [HttpPost("DoanhThuThang")]
        public IActionResult DoanhThuThang(DateTime? startdate, DateTime? endDate)
        {
            if (startdate == null || endDate == null)
            {
                return BadRequest("Ngày bắt đầu và ngày kết thúc không hợp lệ.");
            }

            var hoadons = _context.HoaDons
                .Where(x => x.NgayTao.Date >= startdate && x.NgayTao.Date <= endDate && x.TrangThai == "Đã thanh toán")
                .ToList();

            var doanhThuTheoThang = hoadons
                .GroupBy(x => new { x.NgayTao.Year, x.NgayTao.Month })
                .Select(g => new
                {
                    Thang = $"{g.Key.Month}/{g.Key.Year}",
                    TongTien = g.Sum(x => x.SoTienDaThanhToan)
                })
                .OrderBy(x => x.Thang)
                .ToList();

            var labels = doanhThuTheoThang.Select(x => x.Thang).ToArray();
            var data = doanhThuTheoThang.Select(x => x.TongTien).ToArray();

            var chartData = new
            {
                labels = labels,
                data = data
            };

            ViewBag.ChartData = chartData;
            ViewBag.HoaDons = hoadons;

            return PartialView("DoanhThuThang", hoadons);
        }



    }
}
