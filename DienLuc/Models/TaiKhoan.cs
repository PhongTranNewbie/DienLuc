using System;
using System.Collections.Generic;

namespace DienLuc.Models
{
    public partial class TaiKhoan
    {
        public TaiKhoan()
        {
            LoaiTk = "0";
        }

        public string MaTk { get; set; } = null!;
        public string? Mk { get; set; }
        public string LoaiTk { get; set; } = "0";
        public virtual KhachHang? KhachHang { get; set; }
        public virtual NhanVien? NhanVien { get; set; }
    }
}
