using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string Matk { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string Ho { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string DiaChi { get; set; } = null!;

    public string SoDt { get; set; } = null!;

    public string MaThue { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<DongHoDien> DongHoDiens { get; set; } = new List<DongHoDien>();

    public virtual TaiKhoan MatkNavigation { get; set; } = null!;
}