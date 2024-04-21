using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using DienLuc;

namespace DienLuc.Controllers
{
    public class ThongBaosController : Controller
    {
        private readonly DienLucContext _context;
        private readonly IEmailSender _emailSender;

        public ThongBaosController(DienLucContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        // GET: ThongBaos
        public async Task<IActionResult> Index()
        {
            var dienLucContext = _context.ThongBaos.Include(t => t.MaHdNavigation);
            return View(await dienLucContext.ToListAsync());
        }

        // GET: ThongBaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos
                .Include(t => t.MaHdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongBao == null)
            {
                return NotFound();
            }

            return View(thongBao);
        }

        // GET: ThongBaos/Create
        public IActionResult Create()
        {
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd");
            return View();
        }

        // POST: ThongBaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaHd,Email,NgayThongBao,NoiDung")] ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongBao);
                await _context.SaveChangesAsync();

                // Gửi email
                await _emailSender.SendEmailAsync(thongBao.Email, "New Thong Bao Created", $"MaHd: {thongBao.MaHd}, NgayThongBao: {thongBao.NgayThongBao}, NoiDung: {thongBao.NoiDung}");

                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", thongBao.MaHd);
            return View(thongBao);
        }

        // GET: ThongBaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
            {
                return NotFound();
            }
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", thongBao.MaHd);
            return View(thongBao);
        }

        // POST: ThongBaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaHd,Email,NgayThongBao,NoiDung")] ThongBao thongBao)
        {
            if (id != thongBao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongBao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongBaoExists(thongBao.Id))
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
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", thongBao.MaHd);
            return View(thongBao);
        }

        // GET: ThongBaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos
                .Include(t => t.MaHdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongBao == null)
            {
                return NotFound();
            }

            return View(thongBao);
        }

        // POST: ThongBaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao != null)
            {
                _context.ThongBaos.Remove(thongBao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongBaoExists(int id)
        {
            return _context.ThongBaos.Any(e => e.Id == id);
        }
    }
}
