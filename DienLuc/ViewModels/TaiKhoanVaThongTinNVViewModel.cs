using DienLuc.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace DienLuc.ViewModels
{
    public class TaiKhoanVaThongTinNVViewModel
    {

        public string MaNv { get; set; }
        [Required(ErrorMessage = "Mã tài khoản là bắt buộc.")]
        public string MaTk { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Mk { get; set; }

        public string VaiTro { get; set; }
        public string LoaiTK { get; set; }

        [Required(ErrorMessage = "CCCD là bắt buộc.")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD phải có độ dài từ 9 đến 12 ký tự.")]
        public string Cccd { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc.")]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateOnly NgaySinh { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(0\d{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDt { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }
}
