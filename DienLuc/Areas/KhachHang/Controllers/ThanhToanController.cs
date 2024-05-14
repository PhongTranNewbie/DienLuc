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
            ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();

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
        [HttpGet("danhsachhoadon")]

        public IActionResult DanhSachHoaDon(int? page)
        {
            var userid = HttpContext.Session.GetString("MaTk");
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            // Lấy danh sách các hóa đơn cần thanh toán (ví dụ: các hóa đơn chưa được thanh toán hoặc còn nợ)
            
            var dongHoDiens = _context.HoaDons
                .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                    .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid &&
                            x.TrangThai == "Chưa thanh toán")
				.AsNoTracking()
				.OrderBy(x => x.MaHd)
				.ToList();
            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in dongHoDiens)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }
            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model



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
			int pageSize = 4;
			int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dtt = _context.HoaDons
					.Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
					.Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid &&
								x.TrangThai == "Đã thanh toán")
					.AsNoTracking()
					.OrderByDescending(x => x.MaHd)
					.ToList();

            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in dtt)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model
            PagedList<HoaDon> tdt = new PagedList<HoaDon> (dtt, pageNumber, pageSize);
            ViewBag.DHD = _context.DongHoDiens.ToList();
			return View(tdt);
		}
        private decimal Thue(HoaDon hoaDon)
        {
            // Tính số tiền dựa trên các bậc tiêu thụ điện
            decimal soTien = 0;
            var chisotieuthu = hoaDon.ChiSoMoi - hoaDon.ChiSoCu;

            if (chisotieuthu <= 50)
            {
                soTien = (decimal)(chisotieuthu * 1678); // Bậc 1: 0 - 50 kWh
            }
            else if (chisotieuthu <= 100)
            {
                soTien = (decimal)(50 * 1678 + (chisotieuthu - 50) * 1734); // Bậc 2: 50 - 100 kWh
            }
            else if (chisotieuthu <= 200)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + (chisotieuthu - 100) * 2014); // Bậc 3: 100 - 200 kWh
            }
            else if (chisotieuthu <= 300)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + (chisotieuthu - 200) * 2536); // Bậc 4: 200 - 300 kWh
            }
            else if (chisotieuthu <= 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + (chisotieuthu - 300) * 2834); // Bậc 5: 300 - 400 kWh
            }
            else if (chisotieuthu > 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + 100 * 2834 + (chisotieuthu - 400) * 2927); // Bậc 6: 400 - 500 kWh
            }

            // Tính thuế
            decimal thue = 0;
            if (soTien >= 0)
            {
                thue = soTien * 0.08m;
            }

            return thue;
        }
        [HttpGet("toanbohoadon")]
        public IActionResult HoaDon(int? page)
        {
            var userid = HttpContext.Session.GetString("MaTk");
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dtt = _context.HoaDons
                    .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                    .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == userid)
                    .AsNoTracking()
                    .OrderByDescending(x => x.MaHd)
                    .ToList();
            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in dtt)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }
            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model

            PagedList<HoaDon> tdt = new PagedList<HoaDon>(dtt, pageNumber, pageSize);
            ViewBag.DHD = _context.DongHoDiens.ToList ();
            return View(tdt);
        }



        [HttpGet("TBHD")]
        public IActionResult Filter(string mahd, int? page)
        {
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var userid = HttpContext.Session.GetString("MaTk");
            var a = _context.KhachHangs.FirstOrDefault(x => x.Matk == userid);
            if (userid == null)
            {
                return RedirectToAction("Login");
            }

            IQueryable<HoaDon> ct = _context.HoaDons
                .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == a.Matk);
                


            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào



            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(mahd))
            {
                ct = ct.Where(h => h.MaHd.Contains(mahd));
            }

            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in ct)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model

            // Chuyển IQueryable sang List
            var filteredData = ct.AsNoTracking().OrderByDescending(x => x.MaHd).AsEnumerable();
            PagedList<HoaDon> mhd = new PagedList<HoaDon>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView

            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return PartialView("HoaDon", mhd);
        }



        [HttpGet("DongHoDienDTT")]
        public IActionResult FilterDTT(string mahd, int? page)
        {
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var userid = HttpContext.Session.GetString("MaTk");
            var a = _context.KhachHangs.FirstOrDefault(x => x.Matk == userid);
            if (userid == null)
            {
                return RedirectToAction("Login");
            }

            IQueryable<HoaDon> ct = _context.HoaDons
                .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == a.Matk && x.TrangThai == "Đã thanh toán");


            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào



            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(mahd))
            {
                ct = ct.Where(h => h.MaHd.Contains(mahd));
            }
            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in ct)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model


            // Chuyển IQueryable sang List
            var filteredData = ct.AsNoTracking().OrderByDescending(x => x.MaHd).ToList();
            PagedList<HoaDon> mhd = new PagedList<HoaDon>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView

            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return PartialView("HDDaThanhToan", mhd);
        }



        [HttpGet("DongHoDienCTT")]
        public IActionResult FilterCTT(string mahd, int? page)
        {
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var userid = HttpContext.Session.GetString("MaTk");
            var a = _context.KhachHangs.FirstOrDefault(x => x.Matk == userid);
            if (userid == null)
            {
                return RedirectToAction("Login");
            }

            IQueryable<HoaDon> ct = _context.HoaDons
                .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                .Where(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Matk == a.Matk && x.TrangThai == "Chưa thanh toán");

            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào



            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(mahd))
            {
                ct = ct.Where(h => h.MaHd.Contains(mahd));
            }
            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in ct)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model


            // Chuyển IQueryable sang List
            var filteredData = ct.AsNoTracking().OrderByDescending(x => x.MaHd).ToList();
            PagedList<HoaDon> mhd = new PagedList<HoaDon>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView

            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return PartialView("DanhSachHoaDon", mhd);
        }
    }
}
