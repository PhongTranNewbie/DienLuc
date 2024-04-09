using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string? Matk { get; set; }

    public string? Cccd { get; set; }

    public string? Ho { get; set; }

    public string? Ten { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDt { get; set; }

    public string? MaThue { get; set; }

    public string? MaVaiTro { get; set; }

    public virtual ICollection<DongHoDien> DongHoDiens { get; set; } = new List<DongHoDien>();

    public virtual VaiTro? MaVaiTroNavigation { get; set; }

    public virtual TaiKhoan? MatkNavigation { get; set; }
}
