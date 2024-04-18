using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public int MaChiTietSddien { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgayDenHan { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public decimal? SoNo { get; set; }

    public decimal SoTienCanThanhToan { get; set; }

    public decimal? SoTienDaThanhToan { get; set; }

    public string MaPtttoan { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public int ChiSoMoi { get; set; }

    public int ChiSoCu { get; set; }

    public virtual ChiTietSddien MaChiTietSddienNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual PhuongThucThanhToan MaPtttoanNavigation { get; set; } = null!;

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
