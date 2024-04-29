using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string Matk { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string Ho { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string DiaChi { get; set; } = null!;

    public string SoDt { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? VaiTro { get; set; }

    public virtual ICollection<ChiTietSddien> ChiTietSddiens { get; set; } = new List<ChiTietSddien>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan MatkNavigation { get; set; } = null!;

    public virtual VaiTro? VaiTroNavigation { get; set; }
}