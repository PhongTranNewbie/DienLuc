using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace DienLuc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang")]
    [Route("KhachHang/GiaTien")]
    public class GiaTienController : Controller
    {
        private readonly string priceFilePath = "giadien.txt";
        private readonly string taxFilePath = "tax.txt";
        [HttpGet("TinhTienDien")]
        public IActionResult TienDien()
        {
            return View();
        }
        [HttpPost("TinhTienDien")]
        public IActionResult TienDien(int? chisotieuthu)
        {
            if (chisotieuthu != null)
            {
                decimal? thue = ReadTaxFromFile();
                ViewBag.Thue = thue;

                List<BacThang> bacs = ReadBacThangFromFile();
                ViewBag.Bacs = bacs;

                decimal soTien = TinhTienDien(chisotieuthu.Value, bacs);
                ViewBag.SoTien = soTien;

                if (thue != null)
                {
                    decimal thueTien = soTien * (thue.Value / 100);
                    ViewBag.ThueTien = thueTien;
                    ViewBag.TongTien = soTien + thueTien;
                }
            }

            ViewBag.Chisotieuthu = chisotieuthu;
            return View();
        }

        private decimal TinhTienDien(int chisotieuthu, List<BacThang> bacs)
        {
            decimal soTien = 0;

            foreach (var bac in bacs)
            {
                if (chisotieuthu <= bac.Max)
                {
                    soTien += chisotieuthu * bac.Dongia;
                    break;
                }
                else
                {
                    soTien += (bac.Max - bac.Min + 1) * bac.Dongia;
                    chisotieuthu -= (bac.Max - bac.Min + 1);
                }
            }

            return soTien;
        }

        private List<BacThang> ReadBacThangFromFile()
        {
            List<BacThang> bacs = new List<BacThang>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(priceFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    string[] range = parts[0].Split('-');
                    int min = int.Parse(range[0]);
                    int max = range[1] == "inf" ? int.MaxValue : int.Parse(range[1]);
                    decimal dongia = decimal.Parse(parts[1]);
                    bacs.Add(new BacThang { Min = min, Max = max, Dongia = dongia });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi đọc tệp giá điện
                Console.WriteLine("Error reading price file: " + ex.Message);
            }

            return bacs;
        }

        private decimal? ReadTaxFromFile()
        {
            try
            {
                if (System.IO.File.Exists(taxFilePath))
                {
                    string taxStr = System.IO.File.ReadAllText(taxFilePath);
                    if (decimal.TryParse(taxStr, out decimal tax))
                    {
                        return tax;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi đọc tệp thuế
                Console.WriteLine("Error reading tax file: " + ex.Message);
            }

            return null;
        }

        public IActionResult Index()
        {
            return View();
        }
    }

    public class BacThang
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public decimal Dongia { get; set; }
    }
}
