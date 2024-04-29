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

                        return RedirectToAction("Index", "DongHoDiens");
                    }
                    else if (nhanvien != null)
                    {
                        HttpContext.Session.SetString("MaTk", nhanvien.MaTk);
                        HttpContext.Session.SetString("Role", nhanvien.LoaiTk);

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

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(TaiKhoan taikhoan)
        {
            if (ModelState.IsValid)
            {
                // Assuming here that you have logic to add a new user to the database
                _db.TaiKhoans.Add(taikhoan);
                _db.SaveChanges();

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            return View(taikhoan);
        }

        [HttpGet("DanhSachNV")]
        public IActionResult DanhSachNV(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lnv = _db.NhanViens.AsNoTracking().OrderByDescending(x => x.MaNv).Include(c => c.MatkNavigation).Include(c => c.VaiTroNavigation);

            PagedList<NhanVien> nv = new PagedList<NhanVien>(lnv, pageNumber, pageSize);
            return View(nv);
        }

        [HttpGet("DanhSachKH")]
        public IActionResult DanhSachKH(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lnv = _db.KhachHangs.AsNoTracking().OrderByDescending(x => x.MaKh).Include(c => c.MatkNavigation);

            PagedList<KhachHang> kh = new PagedList<KhachHang>(lnv, pageNumber, pageSize);
            return View(kh);
        }
    }
}
