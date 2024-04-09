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
    public class HoaDonsController : Controller
    {
        private readonly DienLucContext _context;

        public HoaDonsController(DienLucContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var dienLucContext = _context.HoaDons.Include(h => h.MaChiTietSddienNavigation).Include(h => h.MaNvNavigation);
            ViewBag.DHD = _context.DongHoDiens.ToList();
            return View(await dienLucContext.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> GetChiSoThangTruoc(int maChiTietSddien)
        {
            var chiTiet = await _context.ChiTietSddiens
                .Where(c => c.Id == maChiTietSddien)
                .FirstOrDefaultAsync();
            if(chiTiet != null)
            {
                var Dh = chiTiet.MaDh;
                if (Dh != null)
                {
                        var hoaDonThangTruoc = await _context.HoaDons
                        .Where(hd => hd.MaChiTietSddienNavigation != null && hd.MaChiTietSddienNavigation.MaDh == Dh)
                        .OrderByDescending(c => c.NgayTao)
                        .FirstOrDefaultAsync();

                    var chiSoCu = hoaDonThangTruoc != null ? hoaDonThangTruoc.ChiSoMoi : 0;

                    // Lấy chi tiết sử dụng điện của tháng trước (nếu có)


                    // Lấy chỉ số mới của tháng hiện tại (nếu có)
                    var chiSoMoi = hoaDonThangTruoc != null ? chiTiet.ChiSoDien : chiTiet?.ChiSoDien ?? 0;


                    // Tính toán chỉ số tiêu thụ
                    var chiSoTieuThu = chiSoMoi - chiSoCu;
                    return Json(new { chiSoCu = chiSoCu, chiSoMoi = chiSoMoi, chiSoTieuThu = chiSoTieuThu });
                }
            }
            

            

            return Json("Không tìm thấy chỉ số tháng trước");
            
        }


        [HttpGet]
        public async Task<IActionResult> GetMeterAndCustomerCode(int maChiTietSddien)
        {
            // Query the database to get meter code and customer code based on maChiTietSddien
            var chiTietSddien = await _context.ChiTietSddiens
                .Include(ct => ct.MaDhNavigation)
                .FirstOrDefaultAsync(ct => ct.Id == maChiTietSddien);

            if (chiTietSddien == null)
            {
                return NotFound();
            }

            var meterAndCustomerCode = new { maKh = chiTietSddien.MaDhNavigation?.MaKh, maDh = chiTietSddien.MaDhNavigation?.MaDh };
            return Json(meterAndCustomerCode);
        }


        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaChiTietSddienNavigation)
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
        [HttpGet]
        public IActionResult GetChiSoCu(string maDh)
        {
            var chiSoCu = _context.HoaDons
                                .Where(c => c.MaChiTietSddienNavigation.MaDh == maDh)
                                .OrderByDescending(c => c.NgayTao)
                                .Select(c => c.ChiSoMoi)
                                .FirstOrDefault();
            return Json(chiSoCu);
        }
        // GET: HoaDons/Create
        public IActionResult Create()
        {

            ViewData["MaChiTietSddien"] = new SelectList(_context.ChiTietSddiens, "Id", "Id");
            
            return View();
        }
        private decimal TinhTien(HoaDon hoaDon)
        {
            decimal? soTien = 0;
             // Chuyển % thuế thành số thập phân

            // Tính tiền theo bậc
            if (hoaDon.ChiSoTieuThu <= 50)
            {
                soTien = hoaDon.ChiSoTieuThu * 1678; // Bậc 1: 0 - 50 kWh
            }
            else if (hoaDon.ChiSoTieuThu <= 100)
            {
                soTien = 50 * 1678 + (hoaDon.ChiSoTieuThu - 50) * 1734; // Bậc 2: 50 - 100 kWh
            }
            else if (hoaDon.ChiSoTieuThu <= 200)
            {
                soTien = 50 * 1678 + 50 * 1734 + (hoaDon.ChiSoTieuThu - 100) * 2014; // Bậc 3: 100 - 200 kWh
            }
            else if (hoaDon.ChiSoTieuThu <= 300)
            {
                soTien = 50 * 1678 + 50 * 1734 + 100 * 2014 + (hoaDon.ChiSoTieuThu - 200) * 2536; // Bậc 4: 200 - 300 kWh
            }
            else if (hoaDon.ChiSoTieuThu <= 400)
            {
                soTien = 50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + (hoaDon.ChiSoTieuThu - 300) * 2834; // Bậc 5: 300 - 400 kWh
            }
            else
            {
                soTien = 50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + 100 * 2834 + (hoaDon.ChiSoTieuThu - 400) * 2927; // Bậc 6: 400 - 500 kWh
            }

            

            return (decimal)soTien;
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,MaNv,MaKh,MaChiTietSddien,NgayTao,NgayDenHan,NgayThanhToan,Thue,SoNo,SoTienCanThanhToan,SoTienDaThanhToan,HinhThucThanhToan,TrangThai,ChiSoCu,ChiSoMoi,ChiSoTieuThu")] HoaDon hoaDon)
        {
            try
            {
                var exist = await _context.HoaDons.FirstOrDefaultAsync(c => c.MaChiTietSddien == hoaDon.MaChiTietSddien);
                if (exist != null)
                {
                    ModelState.AddModelError(string.Empty, "Hóa đơn cho chi tiết sử dụng điện này đã tồn tại");
                    PopulateDropDownLists(hoaDon);
                    return View(hoaDon);
                }

                // Kiểm tra ngày tạo có hợp lệ hay không
                if (hoaDon.NgayTao > hoaDon.NgayDenHan)
                {
                    ModelState.AddModelError(string.Empty, "Ngày tạo không được lớn hơn ngày đến hạn");
                    PopulateDropDownLists(hoaDon);
                    return View(hoaDon);
                }

                // Lấy chi tiết sử dụng điện
                var chiTiet = await _context.ChiTietSddiens
                    .Where(c => c.Id == hoaDon.MaChiTietSddien)
                    .OrderByDescending(c => c.NgayLayThongTin)
                    .FirstOrDefaultAsync();

                if (chiTiet == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy chi tiết sử dụng điện");
                    PopulateDropDownLists(hoaDon);
                    return View(hoaDon);
                }

                // Lấy hóa đơn gần nhất của đồng hồ

                hoaDon.SoTienCanThanhToan = TinhTien(hoaDon);
                // Thêm hóa đơn vào context và lưu thay đổi
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                PopulateDropDownLists(hoaDon);
                return View(hoaDon);
            }
        }

        private void PopulateDropDownLists(HoaDon hoaDon)
        {
            ViewBag.MaDh = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", hoaDon.MaChiTietSddienNavigation?.MaDh);
            ViewBag.MaNv = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            ViewBag.MaChiTietSddien = new SelectList(_context.ChiTietSddiens, "Id", "Id", hoaDon.MaChiTietSddien);
        }



        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            // Lấy giá trị các trường MaChiTietSddien, MaKh, MaDh, MaNv từ dữ liệu trong hóa đơn
            var maChiTietSddien = hoaDon.MaChiTietSddien;
            var maKh = hoaDon.MaChiTietSddienNavigation?.MaDhNavigation?.MaKh;
            // Lấy MaDh từ MaChiTietSddien
            var maDh = _context.ChiTietSddiens.FirstOrDefault(ct => ct.Id == maChiTietSddien)?.MaDh;
            var maNv = hoaDon.MaNv;
            var maK = await _context.DongHoDiens
                            .Where(dh => dh.MaDh == maDh)
                            .Select(dh => dh.MaKh)
                            .FirstOrDefaultAsync();

            ViewBag.Makh = maK;
            // Pass giá trị vào ViewData để sử dụng trong View
            ViewData["MaChiTietSddien"] = maChiTietSddien;
            ViewData["MaKh"] = maK;
            ViewData["MaDh"] = maDh;
            ViewData["MaNv"] = maNv;

            // Nếu cần thiết, bạn có thể gửi dữ liệu khác vào ViewData tại đây

            return View(hoaDon);
        }



        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHd,MaNv,MaKh,MaChiTietSddien,NgayTao,NgayDenHan,NgayThanhToan,Thue,SoNo,SoTienCanThanhToan,SoTienDaThanhToan,HinhThucThanhToan,TrangThai,ChiSoCu,ChiSoMoi,ChiSoTieuThu")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    hoaDon.ChiSoTieuThu = hoaDon.ChiSoMoi - hoaDon.ChiSoCu;
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHd))
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
            ViewData["MaChiTietSddien"] = new SelectList(_context.ChiTietSddiens, "Id", "MaDh", hoaDon.MaChiTietSddien);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaChiTietSddienNavigation)
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(string id)
        {
            return _context.HoaDons.Any(e => e.MaHd == id);
        }
    }
}
