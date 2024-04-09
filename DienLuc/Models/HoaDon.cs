using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public string? MaNv { get; set; }

    public int? MaChiTietSddien { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayDenHan { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public decimal? Thue { get; set; }

    public decimal? SoNo { get; set; }

    public decimal? SoTienCanThanhToan { get; set; }

    public decimal? SoTienDaThanhToan { get; set; }

    public string? HinhThucThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public int? ChiSoTieuThu { get; set; }

    public int? ChiSoCu { get; set; }

    public int? ChiSoMoi { get; set; }

    public virtual ChiTietSddien? MaChiTietSddienNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
