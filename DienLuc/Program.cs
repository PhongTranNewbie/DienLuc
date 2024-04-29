using DienLuc.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using DienLuc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Move the configuration of DbContext here
builder.Services.AddDbContext<DienLucContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session before calling Build()
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IThongBaoService, ThongBaoService>();
builder.Services.AddHostedService<DailyCheckService>();
// Add email configuration and email sender service
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Move the configuration of Session before calling Build()
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=AccountAdmin}/{action=Login}/{id?}");

app.MapAreaControllerRoute(
    name: "Employee",
    areaName: "NhanVien",
    pattern: "Employee/{controller=AccountEmployee}/{action=Login1}/{id?}");

app.MapAreaControllerRoute(
    name: "Admin_Create",
    areaName: "Admin",
    pattern: "Admin/Create",
    defaults: new { controller = "AccountAdmin", action = "SignIn" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "TaiKhoan", action = "Create" }); // Update default route

app.Run();
