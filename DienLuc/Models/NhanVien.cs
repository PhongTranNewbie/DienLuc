using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string? Matk { get; set; }

    public string? Cccd { get; set; }

    public string? Ho { get; set; }

    public string? Ten { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDt { get; set; }

    public virtual ICollection<ChiTietSddien> ChiTietSddiens { get; set; } = new List<ChiTietSddien>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan? MatkNavigation { get; set; }
}
