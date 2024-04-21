using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DienLuc.Models;

public partial class LoaiTaiKhoan
{
    [Required(ErrorMessage = "Mã loại là bắt buộc.")]
    public string MaLoaiTk { get; set; } = null!;
    [Required(ErrorMessage = "Tên loại là bắt buộc.")]
    public string TenLoaiTk { get; set; } = null!;

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
