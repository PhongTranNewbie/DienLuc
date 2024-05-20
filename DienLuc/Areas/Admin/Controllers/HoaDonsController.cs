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
    [Route("admin/Hoadon")]
    public class HoaDonsController : Controller
    {
        private readonly DienLucContext _context;

        public HoaDonsController(DienLucContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        [HttpGet("")]
        [HttpGet("Danhsachhoadon")]
        public IActionResult Index(int? page)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 2;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hoadon = _context.HoaDons
                .AsNoTracking()
                .OrderByDescending(x => x.MaHd)
                .Include(h => h.MaChiTietSddienNavigation)
                .Include(h => h.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation)
                .Include(h => h.MaPtttoanNavigation)
                .ToList(); // Lấy dữ liệu từ cơ sở dữ liệu và chuyển sang danh sách

            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in hoadon)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model

            PagedList<HoaDon> mhd = new PagedList<HoaDon>(hoadon, pageNumber, pageSize);

            return View(mhd);
        }



        [HttpGet("GetChiSoThangTruoc/{maChiTietSddien}")]
        public async Task<IActionResult> GetChiSoThangTruoc(int maChiTietSddien)
        {
            var chiTiet = await _context.ChiTietSddiens
                .Where(c => c.Id == maChiTietSddien)
                .FirstOrDefaultAsync();
            if (chiTiet != null)
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

        [HttpGet("GetMeterAndCustomerCode/{maChiTietSddien}")]
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
        [HttpGet("Tao")]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            var chiTietSddiens = _context.ChiTietSddiens
                .OrderByDescending(c => c.NgayLayThongTin)
                .ToList();
            ViewData["MaChiTietSddien"] = new SelectList(chiTietSddiens, "Id", "Id");

            return View();
        }
        

        private decimal TinhTien(HoaDon hoaDon)
        {
            // Tính số tiền dựa trên các bậc tiêu thụ điện
            decimal soTien = 0;
            var chisotieuthu = hoaDon.ChiSoMoi - hoaDon.ChiSoCu;

            if (chisotieuthu <= 50)
            {
                soTien = (decimal)(chisotieuthu * 1678); // Bậc 1: 0 - 50 kWh
            }
            else if (chisotieuthu <= 100)
            {
                soTien = (decimal)(50 * 1678 + (chisotieuthu - 50) * 1734); // Bậc 2: 50 - 100 kWh
            }
            else if (chisotieuthu <= 200)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + (chisotieuthu - 100) * 2014); // Bậc 3: 100 - 200 kWh
            }
            else if (chisotieuthu <= 300)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + (chisotieuthu - 200) * 2536); // Bậc 4: 200 - 300 kWh
            }
            else if (chisotieuthu <= 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + (chisotieuthu - 300) * 2834); // Bậc 5: 300 - 400 kWh
            }
            else if (chisotieuthu > 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + 100 * 2834 + (chisotieuthu - 400) * 2927); // Bậc 6: 400 - 500 kWh
            }

            // Tính thuế
            decimal thue = 0;
            if(soTien >= 0)
            {
                thue = soTien * 0.08m;
            }
            var dh = _context.ChiTietSddiens
                .Where(x => x.Id == hoaDon.MaChiTietSddien)
                .Select(x => x.MaDh)
                .FirstOrDefault();

            IQueryable<ChiTietSddien> dhdQuery = _context.ChiTietSddiens
                .Where(x => x.MaDh == dh);

            // Kiểm tra số lượng bản ghi
            if (dhdQuery.Count() > 1)
            {
                dhdQuery = dhdQuery.OrderByDescending(x => x.NgayLayThongTin).Skip(1);
            }
            else
            {
                dhdQuery = dhdQuery.OrderByDescending(x => x.NgayLayThongTin);
            }

            var dhd = dhdQuery.FirstOrDefault();

            decimal soNo = 0;
            if (dhd != null)
            {
                var hoaDonMoiNhat = _context.HoaDons
                    .Where(x => x.MaChiTietSddien == dhd.Id)
                    .OrderByDescending(x => x.NgayTao)
                    .FirstOrDefault();

                // Nếu tìm thấy hóa đơn mới nhất
                if (hoaDonMoiNhat != null)
                {
                    // Trả về số nợ bằng số tiền cần thanh toán mới nhất trừ đi số tiền đã thanh toán
                    soNo = hoaDonMoiNhat.SoTienCanThanhToan - (hoaDonMoiNhat.SoTienDaThanhToan);
                }
            }




            // Lấy hóa đơn mới nhất của đồng hồ trước đó


            // Tổng số tiền cần thanh toán bao gồm cả số nợ
            decimal tongSoTien = soTien + thue + soNo;

            return tongSoTien;
        }

        private decimal Thue(HoaDon hoaDon)
        {
            // Tính số tiền dựa trên các bậc tiêu thụ điện
            decimal soTien = 0;
            var chisotieuthu = hoaDon.ChiSoMoi - hoaDon.ChiSoCu;

            if (chisotieuthu <= 50)
            {
                soTien = (decimal)(chisotieuthu * 1678); // Bậc 1: 0 - 50 kWh
            }
            else if (chisotieuthu <= 100)
            {
                soTien = (decimal)(50 * 1678 + (chisotieuthu - 50) * 1734); // Bậc 2: 50 - 100 kWh
            }
            else if (chisotieuthu <= 200)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + (chisotieuthu - 100) * 2014); // Bậc 3: 100 - 200 kWh
            }
            else if (chisotieuthu <= 300)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + (chisotieuthu - 200) * 2536); // Bậc 4: 200 - 300 kWh
            }
            else if (chisotieuthu <= 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + (chisotieuthu - 300) * 2834); // Bậc 5: 300 - 400 kWh
            }
            else if (chisotieuthu > 400)
            {
                soTien = (decimal)(50 * 1678 + 50 * 1734 + 100 * 2014 + 100 * 2536 + 100 * 2834 + (chisotieuthu - 400) * 2927); // Bậc 6: 400 - 500 kWh
            }

            // Tính thuế
            decimal thue = 0;
            if (soTien >= 0)
            {
                thue = soTien * 0.08m;
            }

            return thue;
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more
        // s, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Tao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,MaNv,MaKh,MaChiTietSddien,NgayTao,NgayDenHan,NgayThanhToan,SoNo,SoTienCanThanhToan,SoTienDaThanhToan,MaPtttoan,TrangThai,ChiSoCu,ChiSoMoi")] HoaDon hoaDon)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            var exist = await _context.HoaDons
                .Include(h => h.MaChiTietSddienNavigation)
                .FirstOrDefaultAsync(c => c.MaChiTietSddien == hoaDon.MaChiTietSddien);

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
     
            if (hoaDon != null)
                {
                    hoaDon.SoTienCanThanhToan = TinhTien(hoaDon);
                }
                var n = _context.NhanViens.FirstOrDefault(x => x.Matk == userId);
                hoaDon.MaNv = n.MaNv;
            // Thêm hóa đơn vào context và lưu thay đổi
            var dh = _context.ChiTietSddiens
                .Where(x => x.Id == hoaDon.MaChiTietSddien)
                .Select(x => x.MaDh)
                .FirstOrDefault();

            IQueryable<ChiTietSddien> dhdQuery = _context.ChiTietSddiens
                .Where(x => x.MaDh == dh);

            // Kiểm tra số lượng bản ghi
            if (dhdQuery.Count() > 1)
            {
                dhdQuery = dhdQuery.OrderByDescending(x => x.NgayLayThongTin).Skip(1);
            }
            else
            {
                dhdQuery = dhdQuery.OrderByDescending(x => x.NgayLayThongTin);
            }

            var dhd = dhdQuery.FirstOrDefault();

            decimal soNo = 0;
            if (dhd != null)
            {
                var hoaDonMoiNhat = _context.HoaDons
                    .Where(x => x.MaChiTietSddien == dhd.Id)
                    .OrderByDescending(x => x.NgayTao)
                    .FirstOrDefault();

                // Nếu tìm thấy hóa đơn mới nhất
                if (hoaDonMoiNhat != null)
                {
                    // Trả về số nợ bằng số tiền cần thanh toán mới nhất trừ đi số tiền đã thanh toán
                    hoaDon.SoNo = hoaDonMoiNhat.SoTienCanThanhToan - (hoaDonMoiNhat.SoTienDaThanhToan);
                }
            }
            // Tạo mã khách hàng mới
            var maxMaHd = await _context.HoaDons.MaxAsync(kh => kh.MaHd);
            string newMaNv;

            // Kiểm tra xem maxMaKh có giá trị null không
            if (maxMaHd == null)
            {
                // Nếu maxMaKh là null, gán mã khách hàng đầu tiên là "KH1"
                newMaNv = "HD1";
            }
            else
            {
                // Nếu maxMaKh không phải null, tăng mã khách hàng lên một đơn vị và sử dụng nó cho mã khách hàng mới
                int nextMaKhNumber = int.Parse(maxMaHd.Replace("HD", "")) + 1;
                newMaNv = "HD" + nextMaKhNumber;
            }
            hoaDon.MaHd = newMaNv;
            _context.Add(hoaDon);
                await _context.SaveChangesAsync();
            TempData["successMessage"] = "Đã tạo hóa đơn thành công";
            return RedirectToAction(nameof(Index));
            
        }

        private void PopulateDropDownLists(HoaDon hoaDon)
        {
            ViewBag.MaDh = new SelectList(_context.DongHoDiens, "MaDh", "MaDh", hoaDon.MaChiTietSddienNavigation?.MaDh);
            ViewBag.MaNv = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            var chiTietSddiens =  _context.ChiTietSddiens
                    .OrderBy(c => c.Id)
                    .ToList();

            ViewBag.MaChiTietSddien = new SelectList(chiTietSddiens, "Id", "Id", hoaDon.MaChiTietSddien);
        }



        // GET: HoaDons/Edit/5
        [HttpGet("ChinhSua/{id}")]
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
            ViewData["MaChiTietSddien"] = new SelectList(_context.PhuongThucThanhToans, "MaPttt", "PhuongPhapTt", hoaDon.MaChiTietSddien);
            // Nếu cần thiết, bạn có thể gửi dữ liệu khác vào ViewData tại đây

            return View(hoaDon);
        }



        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("ChinhSua/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHd,MaNv,MaKh,MaChiTietSddien,NgayTao,NgayDenHan,NgayThanhToan,SoNo,SoTienCanThanhToan,SoTienDaThanhToan,MaPtttoan,TrangThai,ChiSoCu,ChiSoMoi")] HoaDon hoaDon)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            if (id != hoaDon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = "Đã chỉnh sửa hóa đơn thành công";
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

       

        // POST: HoaDons/Delete/5
        [HttpGet("Xoa/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            var tb = _context.ThongBaos.Where(x => x.MaHd == id);
            if(tb != null)
            {
                TempData["errorMessage"] = "Không thể xóa hóa đơn do đã có thông báo cho hóa đơn này.";
                return RedirectToAction(nameof(Index));
            }
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            TempData["successMessage"] = "Đã xóa hóa đơn thành công";
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(string id)
        {
            return _context.HoaDons.Any(e => e.MaHd == id);
        }

        [HttpGet("LocChiTietHD")]
        public IActionResult FilterHoaDon(string trangThai, DateTime? fromDate, DateTime? toDate, int? page, string maHoaDon)
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                // Nếu không tìm thấy mã tài khoản, chuyển hướng đến trang đăng nhập hoặc xử lý thông báo lỗi khác
                return RedirectToAction("Login", "Admin"); // Thay "Login" và "Account" bằng tên action và controller thực tế của trang đăng nhập
            }
            int pageSize = 2;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            IQueryable<HoaDon> hoaDons = _context.HoaDons.Include(h => h.MaChiTietSddienNavigation.MaDhNavigation.MaKhNavigation);
            // Lấy các hóa đơn mới nhất cho mỗi đồng hồ còn nợ
            IQueryable<HoaDon> hd = _context.HoaDons
                .Where(h => (h.SoTienCanThanhToan - h.SoTienDaThanhToan) != 0 && h.TrangThai == "Đã thanh toán")
                .GroupBy(h => h.MaChiTietSddienNavigation.MaDhNavigation.MaDh)
                .Select(g => g.OrderByDescending(h => h.NgayTao).FirstOrDefault());

            // Kiểm tra và lọc theo trạng thái nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(trangThai))
            {
                hoaDons = hoaDons.Where(h => h.TrangThai == trangThai || hd.Contains(h));
            }
            

            // Kiểm tra và lọc theo ngày nếu có giá trị truyền vào
            if (fromDate != null)
            {
                hoaDons = hoaDons.Where(h => h.NgayTao.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                hoaDons = hoaDons.Where(h => h.NgayTao.Date <= toDate.Value.Date);
            }

            // Kiểm tra và lọc theo Mã hóa đơn nếu có giá trị truyền vào
            if (!string.IsNullOrEmpty(maHoaDon))
            {
                hoaDons = hoaDons.Where(h => h.MaHd.Contains(maHoaDon));
            }

            // Chuyển IQueryable sang List
            var filteredData = hoaDons.AsNoTracking().OrderByDescending(x => x.MaHd).ToList();

            List<decimal> listThue = new List<decimal>();

            // Tính số thuế cho mỗi hóa đơn và thêm vào danh sách
            foreach (var hoaDon in filteredData)
            {
                decimal thue = Thue(hoaDon);
                listThue.Add(thue);
            }

            // Truyền danh sách thuế vào ViewBag hoặc model
            ViewBag.ListThue = listThue; // Hoặc truyền vào model nếu bạn đang sử dụng model

            PagedList<HoaDon> mhd = new PagedList<HoaDon>(filteredData, pageNumber, pageSize);
            // Truyền danh sách đồng hồ điện và danh sách khách hàng vào ViewBag để sử dụng trong PartialView


            // Trả về PartialView chứa danh sách hóa đơn đã lọc
            return PartialView("Index", mhd);
        }




    }
}
