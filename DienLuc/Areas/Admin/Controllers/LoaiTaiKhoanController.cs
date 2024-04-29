using DienLuc.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DienLuc.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("admin")]
	[Route("admin/ltk")]
	public class LoaiTaiKhoanController : Controller
	{

		private readonly DienLucContext _context;
		public LoaiTaiKhoanController(DienLucContext context)
		{
			_context = context;
		}
		[HttpGet("")]
		[HttpGet("DanhSachLoaiTK")]
		public IActionResult ListAccount()
		{
			var role = _context.LoaiTaiKhoans.ToList();
			return View(role);
		}
		[HttpGet("ThemLoaiTK")]
		public IActionResult CreateAccount()
		{
            /*var userid = HttpContext.Session.GetString("MaTk");
            if (userid == null)
            {
                return NotFound();
            }*/
            return View();
		}
        [HttpPost("ThemLoaiTK")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAccount(LoaiTaiKhoan ltk)
        {
            if (ModelState.IsValid)
            {
                var existingMaLoaiTk = _context.LoaiTaiKhoans.FirstOrDefault(x => x.MaLoaiTk == ltk.MaLoaiTk);
                var existingTenLoaiTk = _context.LoaiTaiKhoans.FirstOrDefault(x => x.TenLoaiTk == ltk.TenLoaiTk);

                if (existingMaLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Mã loại tài khoản này đã tồn tại.");
                }
                else if (existingTenLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên loại tài khoản này đã tồn tại.");
                }
                else
                {
                    _context.Add(ltk);
                    _context.SaveChanges();
                    TempData["successMessage"] = "Đã tạo loại tài khoản thành công";
                    return RedirectToAction("ListAccount");
                }
            }

            return View(ltk);
        }

        
        [HttpGet("DanhSachVaiTro")]
        public IActionResult ListRole()
        {
            var role = _context.VaiTros.ToList();
            return View(role);
        }
        [HttpGet("ThemVaiTro")]
        public IActionResult CreateRole()
        {
            /*var userid = HttpContext.Session.GetString("MaTk");
            if (userid == null)
            {
                return NotFound();
            }*/
            return View();
        }
        [HttpPost("ThemVaiTro")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRole(VaiTro ltk)
        {
            if (ModelState.IsValid)
            {
                var existingMaLoaiTk = _context.VaiTros.FirstOrDefault(x => x.MaVaiTro == ltk.MaVaiTro);
                var existingTenLoaiTk = _context.VaiTros.FirstOrDefault(x => x.ViTri == ltk.ViTri);

                if (existingMaLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Mã vai trò này đã tồn tại.");
                }
                else if (existingTenLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Vị trí này đã tồn tại.");
                }
                else
                {
                    _context.Add(ltk);
                    _context.SaveChanges();
                    TempData["successMessage"] = "Đã tạo vai trò thành công";
                    return RedirectToAction("ListRole");
                }
            }

            return View(ltk);
        }

        [HttpGet("DanhSachPT")]
        public IActionResult DanhSachTT()
        {
            var role = _context.PhuongThucThanhToans.ToList();
            return View(role);
        }
        [HttpGet("ThemPTTT")]
        public IActionResult PTTT()
        {
            /*var userid = HttpContext.Session.GetString("MaTk");
            if (userid == null)
            {
                return NotFound();
            }*/
            return View();
        }
        [HttpPost("ThemPTTT")]
        [ValidateAntiForgeryToken]
        public IActionResult PTTT(PhuongThucThanhToan ltk)
        {
            if (ModelState.IsValid)
            {
                var existingMaLoaiTk = _context.PhuongThucThanhToans.FirstOrDefault(x => x.MaPttt == ltk.MaPttt);
                var existingTenLoaiTk = _context.PhuongThucThanhToans.FirstOrDefault(x => x.PhuongPhapTt == ltk.PhuongPhapTt);

                if (existingMaLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Mã phương thức thanh toán này đã tồn tại.");
                }
                else if (existingTenLoaiTk != null)
                {
                    ModelState.AddModelError(string.Empty, "Phương thức thanh toán này đã tồn tại.");
                }
                else
                {
                    _context.Add(ltk);
                    _context.SaveChanges();
                    TempData["successMessage"] = "Đã tạo phương thức thanh toán thành công";
                    return RedirectToAction("DanhSachTT");
                }
            }

            return View(ltk);
        }

    }
}
