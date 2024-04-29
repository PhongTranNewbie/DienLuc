using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("admin/chitietsddien")]
    public class ChiTietSddiensController : Controller
    {
        private readonly DienLucContext _context;

        public ChiTietSddiensController(DienLucContext context)
        {
            _context = context;
        }

        // GET: ChiTietSddiens
        [HttpGet("")]
        [HttpGet("DanhsachSDDien")]
        public IActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var ctdh = _context.ChiTietSddiens.AsNoTracking().OrderByDescending(x => x.Id).Include(c => c.MaDhNavigation).Include(c => c.MaNvNavigation);
            PagedList<ChiTietSddien> ct = new PagedList<ChiTietSddien>(ctdh, pageNumber, pageSize);
            ViewBag.DHD = _context.DongHoDiens.ToList();
            ViewBag.khachHangList = _context.KhachHangs.ToList();
            return View(ct);

        }

        // GET: ChiTietSddiens/Details/5
        public IActionResult GetChiSoCu(string maDh)
        {
            // Lấy chi tiết sử dụng điện của đồng hồ được chọn cho tháng trước
            var lastMonth = DateTime.Now.AddMonths(-1);
            var chiTietCu = _context.ChiTietSddiens
                .Where(c => c.MaDh == maDh && c.NgayLayThongTin.Year == lastMonth.Year && c.NgayLayThongTin.Month == lastMonth.Month)
                .OrderByDescending(c => c.NgayLayThongTin)
                .FirstOrDefault();

            // Trả về chỉ số điện của tháng trước nếu có, ngược lại trả về 0
            var chiSoCu = chiTietCu != null ? chiTietCu.ChiSoDien : 0;

            return Content(chiSoCu.ToString());
        }
        [HttpGet("ChiTiet/{id}")]
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
        [HttpGet("Tao")]
        public IActionResult Create()
        {
            ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh");
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            return View();
        }

        // POST: ChiTietSddiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Tao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaDh,NgayLayThongTin,MaNv,ChiSoDien")] ChiTietSddien chiTietSddien)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "AccountEmployee"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }


            var maxChiTiet = _context.ChiTietSddiens
        .Where(c => c.MaDh == chiTietSddien.MaDh)
        .OrderByDescending(c => c.NgayLayThongTin)
        .FirstOrDefault();

            // Nếu có chi tiết sử dụng điện trước đó
            if (maxChiTiet != null)
            {
                // Kiểm tra xem chi tiết mới có tháng và năm bé hơn hoặc bằng tháng và năm lớn nhất không
                if (chiTietSddien.NgayLayThongTin.Year < maxChiTiet.NgayLayThongTin.Year ||
                    chiTietSddien.NgayLayThongTin.Year == maxChiTiet.NgayLayThongTin.Year &&
                     chiTietSddien.NgayLayThongTin.Month <= maxChiTiet.NgayLayThongTin.Month)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo chi tiết sử dụng điện cho tháng và năm này.");
                    // Load danh sách đồng hồ và nhân viên để hiển thị lại form
                    ViewBag.MaDh = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
                    ViewBag.MaNv = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
                    return View(chiTietSddien);
                }
            }


            var lastestCsd = _context.ChiTietSddiens.Where(c => c.MaDh == chiTietSddien.MaDh).OrderByDescending(c => c.NgayLayThongTin).Select(c => c.ChiSoDien).FirstOrDefault();
            if (lastestCsd > chiTietSddien.ChiSoDien)
            {
                ModelState.AddModelError(string.Empty, $"Chỉ số điện không được bé hơn chi số điện tháng trước là {lastestCsd}.");
                // Trả về view với model để hiển thị thông báo lỗi cho người dùng
                ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh");
                ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
                return View(chiTietSddien);
            }
            var n = _context.NhanViens.FirstOrDefault(x => x.Matk == userId);
            chiTietSddien.MaNv = n.MaNv;
            _context.Add(chiTietSddien);
            await _context.SaveChangesAsync();
            TempData["successMessage"] = "Đã tạo chi tiết sử dụng điện thành công";
            return RedirectToAction(nameof(Index));

        }

        // GET: ChiTietSddiens/Edit/5
        [HttpGet("ChinhSua/{id}")]
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


        // POST: ChiTietSddiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("ChinhSua/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaDh,NgayLayThongTin,MaNv,ChiSoDien")] ChiTietSddien chiTietSddien)
        {
            if (id != chiTietSddien.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra xem chi tiết sử dụng điện đã được sử dụng để tạo hóa đơn chưa
                var isUsedInInvoice = await _context.HoaDons.AnyAsync(hd => hd.MaChiTietSddien == id);

                if (isUsedInInvoice)
                {
                    ModelState.AddModelError(string.Empty, "Không thể chỉnh sửa chi tiết sử dụng điện này vì đã được sử dụng để tạo hóa đơn.");
                    ViewData["MaDh"] = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", chiTietSddien.MaDh);
                    ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", chiTietSddien.MaNv);
                    return View("Edit", chiTietSddien);
                }
                try
                {

                    _context.Update(chiTietSddien);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = "Đã chỉnh sửa chi tiết sử dụng điện thành công";
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
            TempData["errorMessage"] = "Đã chỉnh sửa chi tiết sử dụng điện thất bại";
            return View(chiTietSddien);
        }

        // GET: ChiTietSddiens/Delete/5
        [HttpGet("Xoa/{id}")]
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
        [HttpPost("Xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(ModelState.IsValid)
            {
                var chiTietSddien = await _context.ChiTietSddiens
                .Include(c => c.MaDhNavigation)
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.Id == id); ;
                // Kiểm tra xem chi tiết sử dụng điện đã được sử dụng để tạo hóa đơn chưa
                var isUsedInInvoice = await _context.HoaDons.AnyAsync(hd => hd.MaChiTietSddien == id);

                if (isUsedInInvoice)
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa chi tiết sử dụng điện này vì đã được sử dụng để tạo hóa đơn.");
                    return View("Delete", chiTietSddien);
                }

                if (chiTietSddien != null)
                {
                    _context.ChiTietSddiens.Remove(chiTietSddien);
                }

                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Đã xóa chi tiết sử dụng điện thành công";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Không thể xóa chi tiết sử dụng điện này vì đã được sử dụng để tạo hóa đơn.");
            TempData["errorMessage"] = "Xóa chi tiết sử dụng điện thất bại";
            return RedirectToAction(nameof(Delete));
        }

        private bool ChiTietSddienExists(int id)
        {
            return _context.ChiTietSddiens.Any(e => e.Id == id);
        }

        [HttpGet("DongHo")]
        public IActionResult Filter(string maKh, int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;


            IQueryable<ChiTietSddien> ct = _context.ChiTietSddiens.Include(h => h.MaDhNavigation.MaKhNavigation);

            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào
            

            
            // Kiểm tra và lọc theo mã khách hàng nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(maKh))
            {
                ct = ct.Where(h => h.MaDhNavigation.MaKh == maKh);
            }


            // Chuyển IQueryable sang List
            var filteredData = ct.AsNoTracking().OrderByDescending(x => x.Id).ToList();
            PagedList<ChiTietSddien> mhd = new PagedList<ChiTietSddien>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView
            ViewBag.DHD = _context.DongHoDiens.ToList();
            ViewBag.khachHangList = _context.KhachHangs.ToList();

            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return View("Index", mhd);
        }

        [HttpGet("LayThongTinTheoNgay")]
        public IActionResult LayThongTinTheoNgay(DateTime ngayLayThongTin, int? page)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;

            // Truy vấn cơ sở dữ liệu để lấy dữ liệu theo ngày lấy thông tin được chọn
            var chiTietSdDienList = _context.ChiTietSddiens
                .Where(x => x.NgayLayThongTin.Date == ngayLayThongTin.Date) // Lọc theo ngày lấy thông tin
                .Include(x => x.MaDhNavigation)
                .OrderByDescending(x => x.Id) // Sắp xếp theo ID hoặc một trường khác nếu cần
                .ToList();

            // Phân trang dữ liệu
            var pagedList = chiTietSdDienList.ToPagedList(pageNumber, pageSize);

            // Trả về view với dữ liệu đã lọc và phân trang
            return View("Index", pagedList);
        }
    }
}
