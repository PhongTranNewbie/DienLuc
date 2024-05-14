using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using DienLuc.Services;
using X.PagedList;

namespace DienLuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/ThongBao")]
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
        [HttpGet("")]
        [HttpGet("DanhSachThongBao")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dienLucContext = _context.ThongBaos.AsNoTracking().Include(t => t.MaHdNavigation).ToList();
            PagedList<ThongBao> ct = new PagedList<ThongBao>(dienLucContext, pageNumber, pageSize);

            return View(ct);
        }

        // GET: ThongBaos/Details/5
        [HttpGet("ChiTietThongBao")]
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
        [HttpGet("GuiThongBao")]
        public IActionResult Create()
        {
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd");
            return View();
        }

        // POST: ThongBaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("GuiThongBao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaHd,NgayThongBao,NoiDung,NgayHetHan")] ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                thongBao.NgayThongBao = DateOnly.FromDateTime(DateTime.UtcNow);
                _context.Add(thongBao);
                await _context.SaveChangesAsync();

                // Gửi email
                var email = _context.HoaDons
                    .Where(x => x.MaHd == thongBao.MaHd)
                    .Include(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                    .Select(x => x.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation.Email)
                    .FirstOrDefault();
                var tt = _context.ThongBaos.Include(x => x.MaHdNavigation).Where(x => x.MaHd == thongBao.MaHd).Select(x => x.MaHdNavigation.SoTienCanThanhToan).FirstOrDefault();
                await _emailSender.SendEmailAsync(email, "Thông báo đã được gửi",
                    $"Mã hóa đơn: {thongBao.MaHd}\n" +
                    $"Ngày thông báo: {thongBao.NgayThongBao}\n" +
                    $"Ngày hết hạn cho thông báo: {thongBao.NgayHetHan}\n" +
                    $"Nội dung: {thongBao.NoiDung} với số tiền là {tt:N0} VND");




                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", thongBao.MaHd);
            return View(thongBao);
        }

        [HttpGet("NgayHienTai")]
        public IActionResult Date(string id)
        {
            var NgayThongBao = DateOnly.FromDateTime(DateTime.UtcNow);
            return Json(new { NgayThongBao = NgayThongBao });
        }
        private bool ThongBaoExists(int id)
        {
            return _context.ThongBaos.Any(e => e.Id == id);
        }

        [HttpGet("LocChiTiet")]
        public IActionResult LocTheoNgay(string maCT, int? page)
        {
            int pageSize = 2;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            ViewData["pageNumber"] = pageNumber;

            // Truy vấn cơ sở dữ liệu để lấy dữ liệu trong khoảng thời gian từ fromDate đến toDate
            var query = _context.ThongBaos.Include(x => x.MaHdNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(maCT))
            {
                // Tìm kiếm theo MaHd
                query = query.Where(x => x.MaHd.Contains(maCT));
            }



            // Sắp xếp và lấy dữ liệu
            var chiTietSdDienList = query
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToList();

            // Phân trang dữ liệu
            PagedList<ThongBao> ct = new PagedList<ThongBao>(chiTietSdDienList, pageNumber, pageSize);

            // Trả về PartialView với dữ liệu đã lọc và phân trang
            return PartialView("Index", ct);
        }
    }
}
