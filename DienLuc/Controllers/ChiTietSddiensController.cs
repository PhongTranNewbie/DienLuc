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
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens
                .Include(c => c.MaDhNavigation)
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
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
            if (ModelState.IsValid)
            {
                _context.Add(chiTietSddien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
            return View(chiTietSddien);
        }

        // GET: ChiTietSddiens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens.FindAsync(id);
            if (chiTietSddien == null)
            {
                return NotFound();
            }
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
            return View(chiTietSddien);
        }

        // POST: ChiTietSddiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,MaDh,NgayLayThongTin,MaNv,ChiSoCu,ChiSoMoi,ChiSoTieuThu")] ChiTietSddien chiTietSddien)
        {
            if (id != chiTietSddien.MaDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietSddien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietSddienExists(chiTietSddien.MaDh))
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
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietSddien = await _context.ChiTietSddiens
                .Include(c => c.MaDhNavigation)
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (chiTietSddien == null)
            {
                return NotFound();
            }

            return View(chiTietSddien);
        }

        // POST: ChiTietSddiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chiTietSddien = await _context.ChiTietSddiens.FindAsync(id);
            if (chiTietSddien != null)
            {
                _context.ChiTietSddiens.Remove(chiTietSddien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietSddienExists(string id)
        {
            return _context.ChiTietSddiens.Any(e => e.MaDh == id);
        }
    }
}
