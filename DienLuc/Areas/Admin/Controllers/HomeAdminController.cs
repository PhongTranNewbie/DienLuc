using DienLuc.Areas.KhachHang.Controllers;
using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Dienluc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        private readonly DienLucContext _context;
        private readonly string priceFilePath = "giadien.txt";
        private readonly string taxFilePath = "tax.txt";
        public HomeAdminController(DienLucContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        [HttpGet("Trangchu")]
        public IActionResult Index()
        {
            ViewBag.DHD = _context.DongHoDiens.Count();
            ViewBag.HD = _context.HoaDons.Count();
            ViewBag.HDCTT = _context.HoaDons.Where(x => x.TrangThai == "Chưa thanh toán").Count();
            ViewBag.HDCTT = _context.HoaDons.Where(x => x.TrangThai == "Đã thanh toán").Count();
            ViewBag.NV = _context.NhanViens.Count();
            ViewBag.KH = _context.KhachHangs.Count();
            return View();
        }



        [HttpGet("thanhtoan")]
        public IActionResult ThanhToan(string maHd)
        {
            var hoaDon = _context.HoaDons.Include(x => x.MaPtttoanNavigation).FirstOrDefault(c => c.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();
            return View(hoaDon);
        }

        [HttpPost("thanhtoan")]
        public IActionResult ThanhToan(string maHd, decimal SoTienThanhToan, string pttt)
        {
            var hoaDon = _context.HoaDons.FirstOrDefault(c => c.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu đồng hồ có mã hóa đơn trước đó là còn nợ và đã chuyển sang hóa đơn tiếp theo
            var sameID = _context.ChiTietSddiens.FirstOrDefault(c => c.Id == hoaDon.MaChiTietSddien);

            var maDh = sameID?.MaDh;
            var hoaDonTruoc = _context.HoaDons
                                    .Where(h => h.MaChiTietSddienNavigation != null
                                             && h.MaChiTietSddienNavigation.MaDh == maDh)


                                    .OrderBy(h => h.NgayTao)
                                    .Take(_context.HoaDons.Count() - 1)
                                    .LastOrDefault();

            hoaDon.NgayThanhToan = DateTime.Now;
            hoaDon.SoTienDaThanhToan = SoTienThanhToan;
            hoaDon.MaPtttoan = pttt;
            if (hoaDon.SoTienDaThanhToan <= 0)
            {
                ModelState.AddModelError(string.Empty, "Bạn không thể thanh toán nếu số tiền bé hơn hoặc bằng 0");
                ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();

                return View(hoaDon);

            }
            if (hoaDon.SoTienDaThanhToan > hoaDon.SoTienCanThanhToan)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã thanh toán vượt quá số tiền cần thanh toán");
                ViewBag.PTTT = _context.PhuongThucThanhToans.ToList();

                return View(hoaDon);

            }
            // Tính tiền điện
            int? chisotieuthu = hoaDon.ChiSoMoi - hoaDon.ChiSoCu;
            if (chisotieuthu != null)
            {
                decimal? thue = ReadTaxFromFile();
                List<BacThang> bacs = ReadBacThangFromFile();

                decimal soTienTruocThue = TinhTienDien(chisotieuthu.Value, bacs);
                ViewBag.SoTienTruocThue = soTienTruocThue;

                decimal thueTien = 0;
                decimal tongTien = soTienTruocThue;

                if (thue != null)
                {
                    thueTien = soTienTruocThue * (thue.Value / 100);
                    ViewBag.ThueTien = thueTien;
                    tongTien = soTienTruocThue + thueTien;

                }

                ViewBag.SoTienDien = tongTien;
            }
            hoaDon.TrangThai = "Đã thanh toán";
            _context.SaveChanges();
            return RedirectToAction(nameof(ThanhToanThanhCong));
        }
        [HttpGet("thanhtoanthanhcong")]

        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }

        [HttpGet("dshd")]
        public IActionResult DSHD(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hoadon = _context.HoaDons.Where(x => x.TrangThai == "Chưa thanh toán")
                .AsNoTracking()
                .OrderByDescending(x => x.MaHd)
                .Include(h => h.MaChiTietSddienNavigation)
                .Include(h => h.MaChiTietSddienNavigation.MaDhNavigation);
            PagedList<HoaDon> mhd = new PagedList<HoaDon>(hoadon, pageNumber, pageSize);
            ViewBag.DHD = _context.DongHoDiens.ToList();
            ViewBag.khachHangList = _context.KhachHangs.ToList();
            return View(mhd);
        }

        [HttpPost("TinhTienDien")]
        public IActionResult TienDien(int? chisotieuthu)
        {
            if (chisotieuthu != null)
            {
                decimal? thue = ReadTaxFromFile();
                ViewBag.Thue = thue;

                List<BacThang> bacs = ReadBacThangFromFile();
                

                decimal soTien = TinhTienDien(chisotieuthu.Value, bacs);
                ViewBag.SoTien = soTien;

                if (thue != null)
                {
                    decimal thueTien = soTien * (thue.Value / 100);
                    ViewBag.ThueTien = thueTien;
                    ViewBag.TongTien = soTien + thueTien;
                }
            }

            ViewBag.Chisotieuthu = chisotieuthu;
            return View();
        }

        private decimal TinhTienDien(int chisotieuthu, List<BacThang> bacs)
        {
            decimal soTien = 0;

            foreach (var bac in bacs)
            {
                if (chisotieuthu <= bac.Max)
                {
                    soTien += chisotieuthu * bac.Dongia;
                    break;
                }
                else
                {
                    soTien += (bac.Max - bac.Min + 1) * bac.Dongia;
                    chisotieuthu -= (bac.Max - bac.Min + 1);
                }
            }

            return soTien;
        }

        private List<BacThang> ReadBacThangFromFile()
        {
            List<BacThang> bacs = new List<BacThang>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(priceFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    string[] range = parts[0].Split('-');
                    int min = int.Parse(range[0]);
                    int max = range[1] == "inf" ? int.MaxValue : int.Parse(range[1]);
                    decimal dongia = decimal.Parse(parts[1]);
                    bacs.Add(new BacThang { Min = min, Max = max, Dongia = dongia });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi đọc tệp giá điện
                Console.WriteLine("Error reading price file: " + ex.Message);
            }

            return bacs;
        }

        private decimal? ReadTaxFromFile()
        {
            try
            {
                if (System.IO.File.Exists(taxFilePath))
                {
                    string taxStr = System.IO.File.ReadAllText(taxFilePath);
                    if (decimal.TryParse(taxStr, out decimal tax))
                    {
                        return tax;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi đọc tệp thuế
                Console.WriteLine("Error reading tax file: " + ex.Message);
            }

            return null;
        }
    }
}
