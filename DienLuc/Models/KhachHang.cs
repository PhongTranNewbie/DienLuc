using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DienLuc.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string Matk { get; set; } = null!;

    [Required(ErrorMessage = "CCCD là bắt buộc")]
    [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD phải có độ dài từ 9 đến 12 ký tự")]
    public string Cccd { get; set; } = null!;

    [Required(ErrorMessage = "Họ là bắt buộc")]
    public string Ho { get; set; } = null!;

    [Required(ErrorMessage = "Tên là bắt buộc")]
    public string Ten { get; set; } = null!;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateOnly NgaySinh { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    public string DiaChi { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDt { get; set; } = null!;

    [Required(ErrorMessage = "Mã thuế là bắt buộc")]
    public string MaThue { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    public virtual ICollection<DongHoDien> DongHoDiens { get; set; } = new List<DongHoDien>();

    public virtual TaiKhoan MatkNavigation { get; set; } = null!;
}