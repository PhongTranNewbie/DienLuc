using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using X.PagedList;

namespace DienLuc.Areas.KhachHang.Controllers
{
    [Area("Khachhang")]
    [Route("Khachhang")]
    [Route("Khachhang/DongHo")]
    public class DongHoController : Controller
    {
        private readonly DienLucContext _context;
        public DongHoController(DienLucContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("danhsachdongho")]
        public IActionResult Index(int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "TaiKhoan"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            var userid = HttpContext.Session.GetString("MaTk");
            var a = _context.KhachHangs.FirstOrDefault(x => x.Matk == userid);
            if (userid == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dH = _context.DongHoDiens.Include(x => x.MaKhNavigation).Where(x => x.MaKh == a.MaKh).AsNoTracking().OrderBy(x => x.MaDh);
            PagedList<DongHoDien> dhd = new PagedList<DongHoDien>(dH, pageNumber, pageSize);
            ViewBag.DongHo = dH;
            return View(dhd);
        }
        [HttpGet("DongHoDien")]
        public IActionResult Filter(string maDongHo, int? page)
        {
            var userid = HttpContext.Session.GetString("MaTk");
            var a = _context.KhachHangs.FirstOrDefault(x => x.Matk == userid);
            if (userid == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Lấy dữ liệu từ cơ sở dữ liệu
            IQueryable<DongHoDien> dongHoData = _context.DongHoDiens.Where(x => x.MaKh == a.MaKh).Include(x => x.MaKhNavigation);

            // Kiểm tra và lọc theo mã đồng hồ nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(maDongHo))
            {
                dongHoData = dongHoData.Where(h => h.MaDh.Contains(maDongHo));
            }

            // Sắp xếp theo MaDh giảm dần và chuyển sang danh sách không theo dõi
            var filteredData = dongHoData.AsNoTracking().OrderByDescending(x => x.MaDh).AsNoTracking().ToList();

            // Tạo đối tượng PagedList để phân trang
            PagedList<DongHoDien> pagedData = new PagedList<DongHoDien>(filteredData, pageNumber, pageSize);

            // Trả về view chứa danh sách đồng hồ đã lọc
            return PartialView("Index", pagedData);
        }

    }
}
