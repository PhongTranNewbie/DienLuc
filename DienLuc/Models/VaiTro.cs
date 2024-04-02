using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class VaiTro
{
    public string Id { get; set; } = null!;

    public string? ViTri { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
