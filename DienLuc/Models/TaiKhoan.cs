using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class TaiKhoan
{
    public string MaTk { get; set; } = null!;

    public string Mk { get; set; } = null!;

    public string LoaiTk { get; set; } = null!;

    public virtual KhachHang? KhachHang { get; set; }

    public virtual LoaiTaiKhoan LoaiTkNavigation { get; set; } = null!;

    public virtual NhanVien? NhanVien { get; set; }
}