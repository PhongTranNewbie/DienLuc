using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;

namespace DienLuc.Controllers
{
    public class ChiTietSddiensController : Controller
    {
        private readonly DienLucContext _context;

        public ChiTietSddiensController(DienLucContext context)
        {
            _context = context;
        }

        // GET: ChiTietSddiens
        public async Task<IActionResult> Index()
        {
            var dienLucContext = _context.ChiTietSddiens.Include(c => c.MaDhNavigation).Include(c => c.MaNvNavigation);
            return View(await dienLucContext.ToListAsync());
        }

        // GET: ChiTietSddiens/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens
                .Include(c => c.MaDhNavigation)
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietSddien == null)
            {
                return NotFound();
            }

            return View(chiTietSddien);
        }

        // GET: ChiTietSddiens/Create
        public IActionResult Create()
        {
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh");
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            return View();
        }

        // POST: ChiTietSddiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaDh,NgayLayThongTin,MaNv,ChiSoCu,ChiSoMoi,ChiSoTieuThu")] ChiTietSddien chiTietSddien)
        {
            var maxChiTiet = _context.ChiTietSddiens
        .Where(c => c.MaDh == chiTietSddien.MaDh)
        .OrderByDescending(c => c.NgayLayThongTin)
        .FirstOrDefault();

            // Nếu có chi tiết sử dụng điện trước đó
            if (maxChiTiet != null)
            {
                // Kiểm tra xem chi tiết mới có tháng và năm bé hơn hoặc bằng tháng và năm lớn nhất không
                if (chiTietSddien.NgayLayThongTin.Year < maxChiTiet.NgayLayThongTin.Year ||
                    (chiTietSddien.NgayLayThongTin.Year == maxChiTiet.NgayLayThongTin.Year &&
                     chiTietSddien.NgayLayThongTin.Month <= maxChiTiet.NgayLayThongTin.Month))
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo chi tiết sử dụng điện cho tháng và năm này.");
                    // Load danh sách đồng hồ và nhân viên để hiển thị lại form
                    ViewBag.MaDh = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
                    ViewBag.MaNv = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
                    return View(chiTietSddien);
                }
            }
            var existingRecord = _context.ChiTietSddiens.FirstOrDefault(c => c.MaDh == chiTietSddien.MaDh
                                                                  && c.NgayLayThongTin.Month == chiTietSddien.NgayLayThongTin.Month
                                                                  && c.NgayLayThongTin.Year == chiTietSddien.NgayLayThongTin.Year);
            if (existingRecord != null)
            {
                ModelState.AddModelError(string.Empty, "Chi tiết sử dụng điện cho mã đồng hồ này trong tháng và năm này đã tồn tại.");
                // Trả về view với model để hiển thị thông báo lỗi cho người dùng
                return View(chiTietSddien);
            }
            chiTietSddien.ChiSoTieuThu = chiTietSddien.ChiSoMoi - chiTietSddien.ChiSoCu;
                _context.Add(chiTietSddien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        // GET: ChiTietSddiens/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens
    .Include(c => c.MaDhNavigation)
    .Include(c => c.MaNvNavigation)
    .FirstOrDefaultAsync(m => m.Id == id); 
            if (chiTietSddien == null)
            {
                return NotFound();
            }
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
            return View(chiTietSddien);
        }
        [HttpGet]
        public IActionResult GetChiSoCu(string maDh)
        {
            var chiSoCu = _context.ChiTietSddiens
                                .Where(c => c.MaDh == maDh)
                                .OrderByDescending(c => c.NgayLayThongTin)
                                .Select(c => c.ChiSoMoi)
                                .FirstOrDefault();
            return Json(chiSoCu);
        }

        // POST: ChiTietSddiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaDh,NgayLayThongTin,MaNv,ChiSoCu,ChiSoMoi,ChiSoTieuThu")] ChiTietSddien chiTietSddien)
        {
            if (id != chiTietSddien.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    chiTietSddien.ChiSoTieuThu = chiTietSddien.ChiSoMoi - chiTietSddien.ChiSoCu;
                    _context.Update(chiTietSddien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietSddienExists(chiTietSddien.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
            return View(chiTietSddien);
        }

        // GET: ChiTietSddiens/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens
                .Include(c => c.MaDhNavigation)
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietSddien == null)
            {
                return NotFound();
            }

            return View(chiTietSddien);
        }

        // POST: ChiTietSddiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietSddien = await _context.ChiTietSddiens
    .Include(c => c.MaDhNavigation)
    .Include(c => c.MaNvNavigation)
    .FirstOrDefaultAsync(m => m.Id == id); ;
            if (chiTietSddien != null)
            {
                _context.ChiTietSddiens.Remove(chiTietSddien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietSddienExists(int id)
        {
            return _context.ChiTietSddiens.Any(e => e.Id == id);
        }
    }
}
