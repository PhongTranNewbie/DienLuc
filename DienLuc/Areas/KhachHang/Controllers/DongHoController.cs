using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using X.PagedList;

namespace DienLuc.Areas.KhachHang.Controllers
{
    [Area("Khachhang")]
    [Route("Khachhang")]
    [Route("Khachhang/DongHo")]
    public class DongHoController : Controller
    {
        private readonly DienLucContext _context;
        public DongHoController(DienLucContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var dH = _context.DongHoDiens.AsNoTracking().OrderBy(x => x.MaDh);
            PagedList<DongHoDien> dhd = new PagedList<DongHoDien>(dH, pageNumber, pageSize);
            ViewBag.DongHo = dH;
            return View(dhd);
        }
        
    }
}
