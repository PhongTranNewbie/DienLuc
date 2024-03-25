using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DienLuc.Models;

public partial class ChiTietSddien
{
    public int Id { get; set; }

    public string MaDh { get; set; } = null!;

    public DateTime NgayLayThongTin { get; set; }

    public string MaNv { get; set; } = null!;
    
    public int? ChiSoCu { get; set; }
    [Required]
    [ChiSoMoiGreaterThanChiSoCu(ErrorMessage = "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.")]
    public int? ChiSoMoi { get; set; }

    public int? ChiSoTieuThu { get; set; }

    public virtual HoaDon? HoaDon { get; set; }

    public virtual DongHoDien MaDhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
