using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using X.PagedList;
using Microsoft.IdentityModel.Tokens;

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
            int pageSize = 2;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dongHoDiens = _context.DongHoDiens.Include(x => x.MaKhNavigation).AsNoTracking().OrderByDescending(x => x.MaDh);
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
                TempData["successMessage"] = "Đã tạo đồng hồ điện thành công";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", dongHoDien.MaKh);
            TempData["errorMessage"] = "Đã tạo đồng hồ điện thất bại";

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
                    TempData["successMessage"] = "Đã chỉnh sửa đồng hồ điện thành công";
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
            TempData["errorMessage"] = "Đã chỉnh sửa đồng hồ điện thất bại";

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
                TempData["successMessage"] = "Đã xóa đồng hồ điện thành công";
            }
            TempData["errorMessage"] = "Đã xóa đồng hồ điện thất bại";

            return RedirectToAction(nameof(Index));
        }

        private bool DongHoDienExists(string id)
        {
            return _context.DongHoDiens.Any(e => e.MaDh == id);
        }

        [HttpGet("DongHoDien")]
        public IActionResult Filter(string trangThai, string LoaiDhd, int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;


            IQueryable<DongHoDien> ct = _context.DongHoDiens.Include(x => x.MaKhNavigation);

            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào



            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(trangThai))
            {
                ct = ct.Where(h => h.TrangThai == trangThai);
            }

            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(LoaiDhd))
            {
                ct = ct.Where(h => h.LoaiDhd == LoaiDhd);
            }

            // Chuyển IQueryable sang List
            var filteredData = ct.AsNoTracking().OrderByDescending(x => x.MaDh).ToList();
            PagedList<DongHoDien> mhd = new PagedList<DongHoDien>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView

            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return View("Index", mhd);
        }
    }
}