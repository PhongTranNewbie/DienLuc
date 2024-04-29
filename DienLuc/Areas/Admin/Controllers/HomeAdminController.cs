using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Dienluc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        private readonly DienLucContext _context;
        public HomeAdminController(DienLucContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        [HttpGet("Trangchu")]
        public IActionResult Index()
        {
            ViewBag.DHD = _context.DongHoDiens.Count();
            ViewBag.HD = _context.HoaDons.Count();
            ViewBag.HDCTT = _context.HoaDons.Where(x => x.TrangThai == "Chưa thanh toán").Count();
            ViewBag.HDCTT = _context.HoaDons.Where(x => x.TrangThai == "Đã thanh toán").Count();
            ViewBag.NV = _context.NhanViens.Count();
            ViewBag.KH = _context.KhachHangs.Count();
            return View();
        }



        [HttpGet("thanhtoan")]
        public IActionResult ThanhToan(string maHd)
        {
            var hoaDon = _context.HoaDons.Include(x => x.MaPtttoanNavigation).FirstOrDefault(c => c.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();
            return View(hoaDon);
        }

        [HttpPost("thanhtoan")]
        public IActionResult ThanhToan(string maHd, decimal SoTienThanhToan, string pttt)
        {
            var hoaDon = _context.HoaDons.FirstOrDefault(c => c.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu đồng hồ có mã hóa đơn trước đó là còn nợ và đã chuyển sang hóa đơn tiếp theo
            var sameID = _context.ChiTietSddiens.FirstOrDefault(c => c.Id == hoaDon.MaChiTietSddien);

            var maDh = sameID?.MaDh;
            var hoaDonTruoc = _context.HoaDons
                                    .Where(h => h.MaChiTietSddienNavigation != null
                                             && h.MaChiTietSddienNavigation.MaDh == maDh)


                                    .OrderBy(h => h.NgayTao)
                                    .Take(_context.HoaDons.Count() - 1)
                                    .LastOrDefault();

            hoaDon.NgayThanhToan = DateTime.Now;
            hoaDon.SoTienDaThanhToan = SoTienThanhToan;
            hoaDon.MaPtttoan = pttt;
            if (hoaDon.SoTienDaThanhToan <= 0)
            {
                ModelState.AddModelError(string.Empty, "Bạn không thể thanh toán nếu số tiền bé hơn hoặc bằng 0");
                ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();

                return View(hoaDon);

            }
            if (hoaDon.SoTienDaThanhToan > hoaDon.SoTienCanThanhToan)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã thanh toán vượt quá số tiền cần thanh toán");
                ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();

                return View(hoaDon);

            }
            hoaDon.TrangThai = "Đã thanh toán";
            _context.SaveChanges();
            return RedirectToAction(nameof(ThanhToanThanhCong));
        }
        [HttpGet("thanhtoanthanhcong")]

        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }

        [HttpGet("dshd")]
        public IActionResult DSHD(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hoadon = _context.HoaDons.Where(x => x.TrangThai == "Chưa thanh toán")
                .AsNoTracking()
                .OrderByDescending(x => x.MaHd)
                .Include(h => h.MaChiTietSddienNavigation)
                .Include(h => h.MaChiTietSddienNavigation.MaDhNavigation);
            PagedList<HoaDon> mhd = new PagedList<HoaDon>(hoadon, pageNumber, pageSize);
            ViewBag.DHD = _context.DongHoDiens.ToList();
            ViewBag.khachHangList = _context.KhachHangs.ToList();
            return View(mhd);
        }
    }
}
