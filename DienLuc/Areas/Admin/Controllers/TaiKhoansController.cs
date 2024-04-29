using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DienLuc.Models;
using DienLuc.ViewModels;

namespace DienLuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Route("admin/taikhoan")]
    public class TaiKhoansController : Controller
    {
        private readonly DienLucContext _context;

        public TaiKhoansController(DienLucContext context)
        {
            _context = context;
        }

        // GET: TaiKhoans
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaiKhoans.ToListAsync());
        }



        [HttpGet("TaoTaiKhoanVaThongTinNV")]
        public IActionResult TaoTaiKhoanVaThongTinNV()
        {
            var user = HttpContext.Session.GetString("MaTk");

            if (user == null)
            {
                return RedirectToAction("Login");
            }
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "ViTri");
            return View();
        }

        [HttpPost("TaoTaiKhoanVaThongTinNV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTaiKhoanVaThongTinNV(TaiKhoanVaThongTinNVViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if(Check(viewModel))
                {
                    ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "ViTri");

                    return View(viewModel);
                }

                // Tạo tài khoản
                var taiKhoan = new TaiKhoan
                {
                    MaTk = viewModel.MaTk,
                    Mk = viewModel.Mk,
                    LoaiTk = "TK2" // Loại tài khoản nhân viên
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();
                // Tạo mã khách hàng mới
                var maxMaNv = await _context.NhanViens.MaxAsync(kh => kh.MaNv);
                string newMaNv;

                // Kiểm tra xem maxMaKh có giá trị null không
                if (maxMaNv == null)
                {
                    // Nếu maxMaKh là null, gán mã khách hàng đầu tiên là "KH1"
                    newMaNv = "NV1";
                }
                else
                {
                    // Nếu maxMaKh không phải null, tăng mã khách hàng lên một đơn vị và sử dụng nó cho mã khách hàng mới
                    int nextMaKhNumber = int.Parse(maxMaNv.Replace("NV", "")) + 1;
                    newMaNv = "NV" + nextMaKhNumber;
                }
                // Tạo đối tượng Nhân viên mới
                var newNhanVien = new NhanVien
                {
                    MaNv = newMaNv,
                    Matk = viewModel.MaTk,
                    Cccd = viewModel.Cccd,
                    Ho = viewModel.Ho,
                    Ten = viewModel.Ten,
                    NgaySinh = viewModel.NgaySinh,
                    DiaChi = viewModel.DiaChi,
                    SoDt = viewModel.SoDt,
                    Email = viewModel.Email,
                    VaiTro = viewModel.VaiTro
                };

                // Thêm nhân viên mới vào context và lưu vào cơ sở dữ liệu
                _context.NhanViens.Add(newNhanVien);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Đã tạo nhân viên thành công";
                return RedirectToAction("Index", "DongHoDiens");
            }
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "ViTri");
            TempData["errorMessage"] = "Đã tạo nhân viên thất bại";
            return View(viewModel);
        }
        public bool Check(TaiKhoanVaThongTinNVViewModel viewModel)
        {
            bool check = false;
            // Kiểm tra trùng lặp Số Điện Thoại
            var existingPhone = _context.NhanViens.Any(x => x.SoDt == viewModel.SoDt);
            if (existingPhone)
            {
                ModelState.AddModelError(nameof(viewModel.SoDt), "Số điện thoại đã được sử dụng cho một nhân viên khác.");
                check = true;
            }

            // Kiểm tra trùng lặp Email
            var existingEmail = _context.NhanViens.Any(x => x.Email == viewModel.Email);
            if (existingEmail)
            {
                ModelState.AddModelError(nameof(viewModel.Email), "Email đã được sử dụng cho một nhân viên khác.");
                check = true;
            }


            // Kiểm tra trùng lặp CCCD
            var existingCCCD =  _context.NhanViens.Any(x => x.Cccd == viewModel.Cccd);
            if (existingCCCD)
            {
                ModelState.AddModelError(nameof(viewModel.Cccd), "CCCD đã được sử dụng cho một nhân viên khác.");
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

        // GET: NhanVien/Details/5
        [HttpGet("ThongTinNV")]
        public async Task<IActionResult> ThongTinNV()
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.Matk == userId);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }
        [HttpGet("ChinhSuaThongTin")]
        public async Task<IActionResult> ChinhSuaThongTin()
        {
            var userId = HttpContext.Session.GetString("MaTk");

            if (userId == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(m => m.Matk == userId);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        [HttpPost("ChinhSuaThongTin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSuaThongTin(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("MaTk");
                if (userId != null)
                {
                    var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(m => m.Matk == userId);
                    if (nhanVien != null)
                    {
                        // Cập nhật thông tin từ model vào đối tượng nhân viên
                        nhanVien.Cccd = model.Cccd;
                        nhanVien.Ho = model.Ho;
                        nhanVien.Ten = model.Ten;
                        nhanVien.NgaySinh = model.NgaySinh;
                        nhanVien.DiaChi = model.DiaChi;
                        nhanVien.SoDt = model.SoDt;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        _context.NhanViens.Update(nhanVien);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("ThongTinNV");
                    }
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view chỉnh sửa với dữ liệu hiện tại
            return View(model);
        }


    }
}
