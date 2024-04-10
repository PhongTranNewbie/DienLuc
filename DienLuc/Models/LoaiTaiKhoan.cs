using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class LoaiTaiKhoan
{
    public int MaLoaiTk { get; set; }

    public string? TenLoaiTk { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
