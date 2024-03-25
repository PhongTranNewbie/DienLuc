using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class DongHoDien
{
    public string MaDh { get; set; } = null!;

    public string? MaKh { get; set; }

    public string? TrangThai { get; set; }

    public DateOnly? NgayCapNhatTrangThai { get; set; }

    public string? LoaiDhd { get; set; }

    public virtual ICollection<ChiTietSddien> ChiTietSddiens { get; set; } = new List<ChiTietSddien>();

    public virtual KhachHang? MaKhNavigation { get; set; }
}
