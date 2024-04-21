using System;
using System.Collections.Generic;

namespace DienLuc.Models;

public partial class ThongBao
{
    public int Id { get; set; }

<<<<<<< HEAD
    public string? MaHd { get; set; }
    public string? Email { get; set; }

    public DateTime? NgayThongBao { get; set; }
=======
    public string MaHd { get; set; } = null!;

    public DateOnly NgayThongBao { get; set; }
>>>>>>> 152607a697303a3d877d99bc8033063ec6db92f4

    public string? NoiDung { get; set; }

    public DateOnly NgayHetHan { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
