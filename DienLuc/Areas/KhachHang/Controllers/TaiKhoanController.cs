using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;

namespace DienLuc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("khachhang")]
    [Route("khachhang/taikhoan")]
    public class TaiKhoanController : Controller
    {
        private readonly DienLucContext _context;

        public TaiKhoanController(DienLucContext context)
        {
            _context = context;
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MaTk") != null || HttpContext.Session.GetString("LoaiTk") != "0")
            {
                // Check if there is no logged-in user or the logged-in user is not an admin
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("Login")]
        public IActionResult Login(TaiKhoan taikhoan)
        {
            if (HttpContext.Session.GetString("MaTk") == null)
            {
                // Kiểm tra xem cả hai trường MaTk và Mk có đều là null hay không
                if (string.IsNullOrEmpty(taikhoan.MaTk) || string.IsNullOrEmpty(taikhoan.Mk))
                {
                    // Nếu một trong hai trường là null, hiển thị thông báo và trả về view
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.");
                    return View(taikhoan);
                }

                var user = _context.TaiKhoans.FirstOrDefault(x => x.MaTk.Equals(taikhoan.MaTk) && x.Mk.Equals(taikhoan.Mk) && x.LoaiTk == "0");

                if (user != null)
                {
                    HttpContext.Session.SetString("MaTk", user.MaTk);
                    HttpContext.Session.SetString("Role", user.LoaiTk);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(taikhoan);
        }

        // GET: TaiKhoan
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaiKhoans.ToListAsync());
        }

        // GET: TaiKhoan/Details/5
        [HttpGet("ChiTiet/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: TaiKhoan/Create
        [HttpGet("TaoTaiKhoan")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("TaoTaiKhoan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTk,Mk,LoaiTk")] TaiKhoan taiKhoan)
        {
            if (string.IsNullOrWhiteSpace(taiKhoan.MaTk) || string.IsNullOrWhiteSpace(taiKhoan.Mk))
            {
                ModelState.AddModelError(string.Empty, "Tên tài khoản và mật khẩu không được để trống.");
            }
            else
            {
                var existingUser = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.MaTk == taiKhoan.MaTk);
                if (existingUser != null)
                {
                    ModelState.AddModelError("MaTk", "Tên tài khoản đã tồn tại. Vui lòng chọn một tên khác.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(taiKhoan);
        }


        // GET: TaiKhoan/Edit/5
        [HttpGet("ChinhSua/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        // POST: TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("ChinhSua/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTk,Mk,LoaiTk")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTk))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");

            }
            return View(taiKhoan);
        }

        // GET: TaiKhoan/Delete/5
        [HttpGet("Xoa/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: TaiKhoan/Delete/5
        [HttpPost("Xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.MaTk == id);
        }
    }
}
