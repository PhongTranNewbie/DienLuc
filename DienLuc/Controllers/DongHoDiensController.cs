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
    public class DongHoDiensController : Controller
    {
        private readonly DienLucContext _context;

        public DongHoDiensController(DienLucContext context)
        {
            _context = context;
        }

        // GET: DongHoDiens
        public async Task<IActionResult> Index()
        {
            var dienLucContext = _context.DongHoDiens.Include(d => d.MaKhNavigation);
            return View(await dienLucContext.ToListAsync());
        }

        // GET: DongHoDiens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dongHoDien = await _context.DongHoDiens
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (dongHoDien == null)
            {
                return NotFound();
            }

            return View(dongHoDien);
        }

        // GET: DongHoDiens/Create
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            return View();
        }

        // POST: DongHoDiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDh,MaKh,TrangThai,NgayCapNhatTrangThai,LoaiDhd")] DongHoDien dongHoDien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dongHoDien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", dongHoDien.MaKh);
            return View(dongHoDien);
        }

        // GET: DongHoDiens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dongHoDien = await _context.DongHoDiens.FindAsync(id);
            if (dongHoDien == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", dongHoDien.MaKh);
            return View(dongHoDien);
        }

        // POST: DongHoDiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDh,MaKh,TrangThai,NgayCapNhatTrangThai,LoaiDhd")] DongHoDien dongHoDien)
        {
            if (id != dongHoDien.MaDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dongHoDien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DongHoDienExists(dongHoDien.MaDh))
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
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", dongHoDien.MaKh);
            return View(dongHoDien);
        }

        // GET: DongHoDiens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dongHoDien = await _context.DongHoDiens
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (dongHoDien == null)
            {
                return NotFound();
            }

            return View(dongHoDien);
        }

        // POST: DongHoDiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dongHoDien = await _context.DongHoDiens.FindAsync(id);
            if (dongHoDien != null)
            {
                _context.DongHoDiens.Remove(dongHoDien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DongHoDienExists(string id)
        {
            return _context.DongHoDiens.Any(e => e.MaDh == id);
        }
    }
}
