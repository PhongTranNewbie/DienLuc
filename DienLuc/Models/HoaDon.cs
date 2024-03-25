using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public string? MaNv { get; set; }

    public string? MaKh { get; set; }

    public int? MaChiTietSddien { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayDenHan { get; set; }

    public string? NgayThanhToan { get; set; }

    public decimal? Thue { get; set; }

    public int? SoNo { get; set; }

    public int? SoTienCanThanhToan { get; set; }

    public int? SoTienDaThanhToan { get; set; }

    public string? HinhThucThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public virtual ChiTietSddien? MaChiTietSddienNavigation { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
