using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace Dienluc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Route("admin/taikhoanad")]
    public class AdminController : Controller
    {
        private readonly DienLucContext _db;

        public AdminController(DienLucContext db)
        {
            _db = db;
        }

        [HttpGet("DangNhap")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MaTk") == null)
            {
                // Check if there is no logged-in user or the logged-in user is not an admin
                return View();
            }
            else
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
        }

        [HttpPost("DangNhap")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(TaiKhoan taikhoan)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("MaTk") == null)
                {
                    var admin = _db.TaiKhoans.FirstOrDefault(x => x.MaTk.Equals(taikhoan.MaTk) && x.Mk.Equals(taikhoan.Mk) && x.LoaiTk == "TK1");
                    var nhanvien = _db.TaiKhoans.FirstOrDefault(x => x.MaTk.Equals(taikhoan.MaTk) && x.Mk.Equals(taikhoan.Mk) && x.LoaiTk == "TK2");
                    if (admin != null)
                    {
                        HttpContext.Session.SetString("MaTk", admin.MaTk);
                        HttpContext.Session.SetString("Role", admin.LoaiTk);
                        TempData["successMessage"] = "Đã đăng nhập thành công";

                        return RedirectToAction("Index", "DongHoDiens");
                    }
                    else if (nhanvien != null)
                    {
                        HttpContext.Session.SetString("MaTk", nhanvien.MaTk);
                        HttpContext.Session.SetString("Role", nhanvien.LoaiTk);
                        TempData["successMessage"] = "Đã đăng nhập thành công";

                        return RedirectToAction("Index", "DongHoDiens");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu."); // Add validation message
                    }
                }

            }

            return View(taikhoan);
        }

        [HttpGet("DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MaTk");
            HttpContext.Session.Remove("Role");

            return RedirectToAction("Login");
        }

        

        [HttpGet("DanhSachNV")]
        public IActionResult DanhSachNV(int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lnv = _db.NhanViens.AsNoTracking().OrderByDescending(x => x.MaNv).Include(c => c.MatkNavigation).Include(c => c.VaiTroNavigation);

            PagedList<NhanVien> nv = new PagedList<NhanVien>(lnv, pageNumber, pageSize);
            return View(nv);
        }

        [HttpGet("DanhSachKH")]
        public IActionResult DanhSachKH(int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lnv = _db.KhachHangs.AsNoTracking().OrderByDescending(x => x.MaKh).Include(c => c.MatkNavigation);

            PagedList<KhachHang> kh = new PagedList<KhachHang>(lnv, pageNumber, pageSize);
            return View(kh);
        }
        [HttpGet("FilterByMaNhanVien")]
        public IActionResult Filter(string maNhanVien, int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Lấy dữ liệu từ cơ sở dữ liệu
            IQueryable<NhanVien> nhanVienData = _db.NhanViens.Include(x => x.MatkNavigation).Include(x => x.VaiTroNavigation);

            

            // Kiểm tra và lọc theo mã đồng hồ nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(maNhanVien))
            {
                nhanVienData = nhanVienData.Where(h => h.MaNv.Contains(maNhanVien) || h.Ho.Contains(maNhanVien) || h.Ten.Contains(maNhanVien));

            }


            // Sắp xếp theo MaDh giảm dần và chuyển sang danh sách không theo dõi
            var filteredData = nhanVienData.AsNoTracking().OrderByDescending(x => x.MaNv).AsNoTracking().ToList();

            // Tạo đối tượng PagedList để phân trang
            PagedList<NhanVien> pagedData = new PagedList<NhanVien>(filteredData, pageNumber, pageSize);

            // Trả về view chứa danh sách đồng hồ đã lọc
            return PartialView("DanhSachNV", pagedData);
        }

        [HttpGet("FilterByMaKH")]
        public IActionResult FilterKH(string maNhanVien, int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Lấy dữ liệu từ cơ sở dữ liệu
            IQueryable<KhachHang> khData = _db.KhachHangs.Include(x => x.MatkNavigation);



            // Kiểm tra và lọc theo mã đồng hồ nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(maNhanVien))
            {
                khData = khData.Where(h => h.MaKh.Contains(maNhanVien) || h.Ho.Contains(maNhanVien) || h.Ten.Contains(maNhanVien));

            }


            // Sắp xếp theo MaDh giảm dần và chuyển sang danh sách không theo dõi
            var filteredData = khData.AsNoTracking().OrderByDescending(x => x.MaKh).AsNoTracking().ToList();

            // Tạo đối tượng PagedList để phân trang
            PagedList<KhachHang> pagedData = new PagedList<KhachHang>(filteredData, pageNumber, pageSize);

            // Trả về view chứa danh sách đồng hồ đã lọc
            return PartialView("DanhSachKH", pagedData);
        }
    }
}
