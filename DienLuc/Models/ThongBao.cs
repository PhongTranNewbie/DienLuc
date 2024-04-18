using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class ThongBao
{
    public int Id { get; set; }

    public string MaHd { get; set; } = null!;

    public DateOnly NgayThongBao { get; set; }

    public string? NoiDung { get; set; }

    public DateOnly NgayHetHan { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
