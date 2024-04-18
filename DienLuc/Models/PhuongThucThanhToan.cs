using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class PhuongThucThanhToan
{
    public string MaPttt { get; set; } = null!;

    public string PhuongPhapTt { get; set; } = null!;

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
