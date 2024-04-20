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

			hoaDon.NgayThanhToan = DateTime.Now;
			hoaDon.SoTienDaThanhToan = SoTienThanhToan;
            if(hoaDon.SoTienDaThanhToan <= 0)
            {
                ModelState.AddModelError(string.Empty, "Bạn không thể thanh toán nếu số tiền bé hơn hoặc bằng 0");
                return View(hoaDon);

            }
			hoaDon.TrangThai = "Đã thanh toán";

			_context.SaveChanges();

			return RedirectToAction(nameof(ThanhToanThanhCong));
		}
        [HttpGet("danhsachhoadon")]

        public IActionResult DanhSachHoaDon(int? page)
        {
            var userid = HttpContext.Session.GetString("MaTk");
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            // Lấy danh sách các hóa đơn cần thanh toán (ví dụ: các hóa đơn chưa được thanh toán hoặc còn nợ)
            
            var dongHoDiens = _context.HoaDons
                .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                    .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid &&
                            x.TrangThai == "Chưa thanh toán")
				.AsNoTracking()
				.OrderBy(x => x.MaHd)
				.ToList();




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
			var userid = HttpContext.Session.GetString("MaTk");
			int pageSize = 12;
			int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dtt = _context.HoaDons
					.Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
					.Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid &&
								x.TrangThai == "Đã thanh toán")
					.AsNoTracking()
					.OrderByDescending(x => x.MaHd)
					.ToList();

            PagedList<HoaDon> tdt = new PagedList<HoaDon> (dtt, pageNumber, pageSize);
			return View(tdt);
		}

        [HttpGet("toanbohoadon")]
        public IActionResult HoaDon(int? page)
        {
            var userid = HttpContext.Session.GetString("MaTk");
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dtt = _context.HoaDons
                    .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                    .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid)
                    .AsNoTracking()
                    .OrderByDescending(x => x.MaHd)
                    .ToList();

            PagedList<HoaDon> tdt = new PagedList<HoaDon>(dtt, pageNumber, pageSize);
            return View(tdt);
        }
    }
}
