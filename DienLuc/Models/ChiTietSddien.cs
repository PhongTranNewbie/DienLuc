using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class ChiTietSddien
{
    public int Id { get; set; }

    public string MaDh { get; set; }

    public DateTime NgayLayThongTin { get; set; }

    public string MaNv { get; set; }

    public int ChiSoDien { get; set; }

    public virtual HoaDon HoaDon { get; set; }

    public virtual DongHoDien MaDhNavigation { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; }
}
