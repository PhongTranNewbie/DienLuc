using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace DienLuc.Controllers
{
	[Area("khachhang")]
	[Route("khachHang")]
	[Route("khachhang/thanhtoan")]
	public class ThanhToanController : Controller
	{
		private readonly DienLucContext _context;
		
		public ThanhToanController(DienLucContext context)
		{
			_context = context;
		}
        [Route("")]
        [Route("index")]
        public IActionResult Index()
		{
			return View();
		}
        [HttpGet("chitietdhd")]

        public IActionResult ChiTietDHD()
		{

			return View();
		}

        [HttpGet("thanhtoan")]
        public IActionResult ThanhToan(string maHd)
        {
            var hoaDon = _context.HoaDons.FirstOrDefault(c => c.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        [HttpPost("thanhtoan")]
        public IActionResult ThanhToan(string maHd, decimal SoTienThanhToan)
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


            if (hoaDonTruoc != null && hoaDonTruoc.TrangThai == "Còn nợ"
				 || hoaDonTruoc != null && hoaDonTruoc.TrangThai == "Đã chuyển sang hóa đơn tiếp theo")
			{
				// Bạn có thể xử lý tại đây, ví dụ hiển thị thông báo yêu cầu thanh toán đầy đủ
				ModelState.AddModelError(string.Empty, "Đồng hồ này có hóa đơn trước đó còn nợ và đã chuyển sang hóa đơn mới. Vui lòng thanh toán đầy đủ.");
				return View(nameof(ThanhToan), hoaDon);
			}

			// Kiểm tra nếu hóa đơn trước đó vẫn còn nợ và số tiền thanh toán không đủ
			if (hoaDon != null && hoaDonTruoc != null && hoaDonTruoc.TrangThai == "Còn nợ"
				&& SoTienThanhToan < hoaDon.SoTienCanThanhToan)
			{
				ModelState.AddModelError(string.Empty, "Hóa đơn trước đó vẫn còn nợ và số tiền thanh toán không đủ. Vui lòng thanh toán số tiền đầy đủ.");
				return View(nameof(ThanhToan), hoaDon);
			}

			hoaDon.NgayThanhToan = DateTime.Now;
			hoaDon.SoTienDaThanhToan = SoTienThanhToan;
			hoaDon.SoNo = hoaDon.SoTienCanThanhToan - hoaDon.SoTienDaThanhToan;
			if (hoaDon.SoTienDaThanhToan >= hoaDon.SoTienCanThanhToan)
			{
				hoaDon.TrangThai = "Đã thanh toán";
			}
			else
			{
				hoaDon.TrangThai = "Còn nợ";
			}

			_context.SaveChanges();

			return RedirectToAction(nameof(ThanhToanThanhCong));
		}
        [HttpGet("danhsachhoadon")]

        public IActionResult DanhSachHoaDon(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            // Lấy danh sách các hóa đơn cần thanh toán (ví dụ: các hóa đơn chưa được thanh toán hoặc còn nợ)
            var dongHoDiens = _context.HoaDons
				.Where(h => h.TrangThai != "Đã thanh toán" && h.TrangThai != "Còn nợ" && h.TrangThai != "Đã chuyền vào tháng tiếp theo")
				.AsNoTracking().OrderBy(x => x.MaHd);
            PagedList<HoaDon> dhd = new PagedList<HoaDon>(dongHoDiens, pageNumber, pageSize);

            return View(dhd);
            
            
        }
        [HttpGet("thanhtoanthanhcong")]

        public IActionResult ThanhToanThanhCong()
		{
			return View();
		}
		[HttpGet("DaThanhToan")]
		public IActionResult HDDaThanhToan(int? page)
		{
			int pageSize = 12;
			int pageNumber = page == null || page < 0 ? 1 : page.Value;
			var dtt = _context.HoaDons.Include(x => x.MaChiTietSddienNavigation.MaDhNavigation)
				.Where(x => x.TrangThai != null && x.TrangThai == "Đã thanh toán" 
				|| x.TrangThai != null && x.TrangThai == "còn nợ")
				.AsNoTracking().OrderBy(x => x.MaHd);
			PagedList<HoaDon> tdt = new PagedList<HoaDon> (dtt, pageNumber, pageSize);
			return View(tdt);
		}
	}
}
