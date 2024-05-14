using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DienLuc.Models;

public partial class NhanVien
{
    [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
    public string MaNv { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Matk { get; set; }

    [Required(ErrorMessage = "CCCD là bắt buộc")]
    [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD phải có độ dài từ 9 đến 12 ký tự")]
    public string Cccd { get; set; }

    [Required(ErrorMessage = "Họ là bắt buộc")]
    public string Ho { get; set; }

    [Required(ErrorMessage = "Tên là bắt buộc")]
    public string Ten { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateOnly NgaySinh { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    public string DiaChi { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDt { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }


    public string? VaiTro { get; set; }

    public virtual ICollection<ChiTietSddien> ChiTietSddiens { get; set; } = new List<ChiTietSddien>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan MatkNavigation { get; set; } = null!;

    public virtual VaiTro? VaiTroNavigation { get; set; }
}