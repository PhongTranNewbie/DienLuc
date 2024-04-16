using DienLuc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Dienluc.Areas.Admin.Controllers
{
    public class AccountEmployeeController : Controller
    {
        private readonly DienLucContext _db;

        public AccountEmployeeController(DienLucContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MaTk") == null || HttpContext.Session.GetString("LoaiTk") != "1")
            {
                // Check if there is no logged-in user or the logged-in user is not an admin
                return View();
            }
            else
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
        }

        [HttpPost]
        public IActionResult Login(TaiKhoan taikhoan)
        {
            if (HttpContext.Session.GetString("MaTk") == null)
            {
                var user = _db.TaiKhoans.FirstOrDefault(x => x.MaTk.Equals(taikhoan.MaTk) && x.Mk.Equals(taikhoan.Mk) && x.LoaiTk == "1");

                if (user != null)
                {
                    HttpContext.Session.SetString("MaTk", user.MaTk);
                    HttpContext.Session.SetString("Role", user.LoaiTk);

                    return RedirectToAction("Index", "HomeAdmin");
                }
            }
            return View(taikhoan);
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(TaiKhoan taikhoan)
        {
            if (ModelState.IsValid)
            {
                // Assuming here that you have logic to add a new user to the database
                _db.TaiKhoans.Add(taikhoan);
                _db.SaveChanges();

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            return View(taikhoan);
        }
    }
}
