using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using X.PagedList;

namespace DienLuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Route("admin/dongho")]

    public class DongHoDiensController : Controller
    {
        private readonly DienLucContext _context;

        public DongHoDiensController(DienLucContext context)
        {
            _context = context;
        }

        // GET: admin/DongHoDiens
        [HttpGet("")]
        [HttpGet("Danhsachdongho")]
        public IActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dongHoDiens = _context.DongHoDiens.AsNoTracking().OrderByDescending(x => x.MaDh);
            PagedList<DongHoDien> dhd = new PagedList<DongHoDien>(dongHoDiens, pageNumber, pageSize);

            return View(dhd);
        }

        // GET: admin/DongHoDiens/Details/5
        [HttpGet("chitiet/{id}")]
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

        // GET: admin/DongHoDiens/Create
        [HttpGet("themmoi")]
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            return View();
        }

        // POST: admin/DongHoDiens/Create
        [HttpPost("themmoi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDh,MaKh,TrangThai,NgayHoatDong,LoaiDhd")] DongHoDien dongHoDien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có đồng hồ nào trong cơ sở dữ liệu hay không
                if (!_context.DongHoDiens.Any())
                {
                    // Nếu không có đồng hồ nào trong cơ sở dữ liệu, gán mã đồng hồ đầu tiên là "DH1"
                    dongHoDien.MaDh = "DH1";
                }
                else
                {
                    // Nếu có đồng hồ trong cơ sở dữ liệu, tạo mã đồng hồ mới bằng cách lấy mã đồng hồ lớn nhất và tăng số tiếp theo lên một
                    var maxMaDh = await _context.DongHoDiens.MaxAsync(d => d.MaDh);
                    var nextMaDhNumber = int.Parse(maxMaDh.Replace("DH", "")) + 1;
                    dongHoDien.MaDh = "DH" + nextMaDhNumber;
                }

                // Thêm đồng hồ vào cơ sở dữ liệu
                _context.Add(dongHoDien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", dongHoDien.MaKh);
            return View(dongHoDien);
        }


        // GET: admin/DongHoDiens/Edit/5
        [HttpGet("chinhsua/{id}")]
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

        // POST: admin/DongHoDiens/Edit/5
        [HttpPost("chinhsua/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDh,MaKh,TrangThai,NgayHoatDong,LoaiDhd")] DongHoDien dongHoDien)
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

        // GET: admin/DongHoDiens/Delete/5
        [HttpGet("Xoa/{id}")]
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

        // POST: admin/DongHoDiens/Delete/5
        [HttpPost("Xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dongHoDien = await _context.DongHoDiens.FindAsync(id);
            var existed = await _context.ChiTietSddiens.AnyAsync(c => c.MaDh == id);
            if (existed)
            {
                ModelState.AddModelError(string.Empty, "Không thể xóa vì đã tồn tại trong chi tiết sử dụng điện");
                return View("Delete", dongHoDien);
            }
            if (dongHoDien != null)
            {
                _context.DongHoDiens.Remove(dongHoDien);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DongHoDienExists(string id)
        {
            return _context.DongHoDiens.Any(e => e.MaDh == id);
        }
    }
}