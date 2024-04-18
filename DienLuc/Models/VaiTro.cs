using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class VaiTro
{
    public string MaVaiTro { get; set; } = null!;

    public string? ViTri { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
