using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DienLuc.ViewModels;

namespace DienLuc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("khachhang")]
    [Route("khachhang/taikhoan")]
    public class TaiKhoanController : Controller
    {
        private readonly DienLucContext _context;

        public TaiKhoanController(DienLucContext context)
        {
            _context = context;
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MaTk") != null || HttpContext.Session.GetString("LoaiTk") != "0")
            {
                // Check if there is no logged-in user or the logged-in user is not an admin
                return View();
            }
            else
            {
                return RedirectToAction("Index", "DongHo");
            }
        }
        [HttpPost("Login")]
        public IActionResult Login(TaiKhoan taikhoan)
        {
            if (HttpContext.Session.GetString("MaTk") == null)
            {
                // Kiểm tra xem cả hai trường MaTk và Mk có đều là null hay không
                if (string.IsNullOrEmpty(taikhoan.MaTk) || string.IsNullOrEmpty(taikhoan.Mk))
                {
                    // Nếu một trong hai trường là null, hiển thị thông báo và trả về view
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.");
                    return View(taikhoan);
                }

                var user = _context.TaiKhoans.FirstOrDefault(x => x.MaTk.Equals(taikhoan.MaTk) && x.Mk.Equals(taikhoan.Mk) && x.LoaiTk == "0");

                if (user != null)
                {
                    HttpContext.Session.SetString("MaTk", user.MaTk);
                    HttpContext.Session.SetString("Role", user.LoaiTk);

                    return RedirectToAction("Index", "DongHo");
                }
            }
            return View(taikhoan);
        }



        [HttpGet("DangKy")]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost("DangKy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(TaiKhoanKhachHangViewModel viewModel)
        {

            
            // Nếu ModelState hợp lệ, tiến hành tạo tài khoản và khách hàng mới
            if (ModelState.IsValid)
            {
                if (Check(viewModel))
                {
                    return View(viewModel);
                }
                // Kiểm tra xem email đã tồn tại chưa
                var taiKhoan = new TaiKhoan
                {
                    MaTk = viewModel.MaTk,
                    Mk = viewModel.Mk,
                    LoaiTk = "TK3" // Loại tài khoản khách hàng
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tạo mã khách hàng mới
                var maxMaKh = await _context.KhachHangs.MaxAsync(kh => kh.MaKh);
                string newMaKh;

                // Kiểm tra xem maxMaKh có giá trị null không
                if (maxMaKh == null)
                {
                    // Nếu maxMaKh là null, gán mã khách hàng đầu tiên là "KH1"
                    newMaKh = "KH1";
                }
                else
                {
                    // Nếu maxMaKh không phải null, tăng mã khách hàng lên một đơn vị và sử dụng nó cho mã khách hàng mới
                    int nextMaKhNumber = int.Parse(maxMaKh.Replace("KH", "")) + 1;
                    newMaKh = "KH" + nextMaKhNumber;
                }

                // Tạo đối tượng KhachHang mới
                var newKhachHang = new Models.KhachHang
                {
                    MaKh = newMaKh,
                    Matk = viewModel.MaTk,
                    Cccd = viewModel.Cccd,
                    Ho = viewModel.Ho,
                    Ten = viewModel.Ten,
                    NgaySinh = viewModel.NgaySinh,
                    DiaChi = viewModel.DiaChi,
                    SoDt = viewModel.SoDt,
                    MaThue = viewModel.MaThue,
                    Email = viewModel.Email
                };

                // Thêm khách hàng mới vào context và lưu vào cơ sở dữ liệu
                _context.KhachHangs.Add(newKhachHang);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");

            }
            return View(viewModel);

        }

        public bool Check(TaiKhoanKhachHangViewModel viewModel)
        {
            bool check = false;
            var existingEmail = _context.KhachHangs.Any(kh => kh.Email == viewModel.Email);
            if (existingEmail)
            {
                ModelState.AddModelError(nameof(viewModel.Email), "Email đã được sử dụng.");
                check = true;
            }

            // Kiểm tra xem mã thuế đã tồn tại chưa
            var existingMaThue = _context.KhachHangs.Any(kh => kh.MaThue == viewModel.MaThue);
            if (existingMaThue)
            {
                ModelState.AddModelError(nameof(viewModel.MaThue), "Mã thuế đã tồn tại.");
                check = true;
            }

            // Kiểm tra xem số điện thoại đã tồn tại chưa
            var existingSoDt = _context.KhachHangs.Any(kh => kh.SoDt == viewModel.SoDt);
            if (existingSoDt)
            {
                ModelState.AddModelError(nameof(viewModel.SoDt), "Số điện thoại đã được sử dụng.");
                check = true;
            }

            // Kiểm tra xem CCCD đã tồn tại chưa
            var existingCccd = _context.KhachHangs.Any(kh => kh.Cccd == viewModel.Cccd);
            if (existingCccd)
            {
                ModelState.AddModelError(nameof(viewModel.Cccd), "CCCD đã được sử dụng.");
                check = true;
            }

            // Kiểm tra xem mã tài khoản đã tồn tại chưa
            var existingMaTk = _context.TaiKhoans.Any(tk => tk.MaTk == viewModel.MaTk);
            if (existingMaTk)
            {
                ModelState.AddModelError(nameof(viewModel.MaTk), "Mã tài khoản đã tồn tại.");
                check = true;
            }
            return check;
        }
        [HttpGet("ThongTinKhachHang")]
        public async Task<IActionResult> ThongTinKhachHang()
        {
            var userid = HttpContext.Session.GetString("MaTk");
            // Lấy thông tin khách hàng từ cơ sở dữ liệu dựa trên mã khách hàng
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Matk == userid);

            if (khachHang == null)
            {
                // Nếu không tìm thấy khách hàng, có thể xử lý bằng cách hiển thị thông báo lỗi hoặc chuyển hướng đến trang khác
                return NotFound(); // Trả về trang 404 Not Found
            }

            // Trả về view hiển thị thông tin của khách hàng
            return View(khachHang);
        }

        [HttpGet("DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MaTk");
            HttpContext.Session.Remove("Role");

            return RedirectToAction("Login");
        }


        [HttpGet("ChinhSuaThongTin")]
        public async Task<IActionResult> ChinhSuaThongTin()
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.KhachHangs.FirstOrDefaultAsync(m => m.Matk == userId);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        [HttpPost("ChinhSuaThongTin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSuaThongTin(Models.KhachHang model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("MaTk");
                if (userId != null)
                {
                    var nhanVien = await _context.KhachHangs.FirstOrDefaultAsync(m => m.Matk == userId);
                    if (nhanVien != null)
                    {
                        // Cập nhật thông tin từ model vào đối tượng nhân viên
                        nhanVien.Cccd = model.Cccd;
                        nhanVien.Ho = model.Ho;
                        nhanVien.Ten = model.Ten;
                        nhanVien.NgaySinh = model.NgaySinh;
                        nhanVien.DiaChi = model.DiaChi;
                        nhanVien.SoDt = model.SoDt;
                        nhanVien.MaThue = model.MaThue;
                        // Lưu thay đổi vào cơ sở dữ liệu
                        _context.KhachHangs.Update(nhanVien);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("ThongTinNV");
                    }
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view chỉnh sửa với dữ liệu hiện tại
            return View(model);
        }
        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.MaTk == id);
        }
    }
}
