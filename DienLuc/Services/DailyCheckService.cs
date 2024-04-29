using DienLuc.Models;
using Microsoft.EntityFrameworkCore;

namespace DienLuc.Services
{
    public class DailyCheckService : BackgroundService
    {
        private readonly IThongBaoService _thongBaoService;
        private readonly DienLucContext _context;

        public DailyCheckService(IThongBaoService thongBaoService, DienLucContext context)
        {
            _thongBaoService = thongBaoService;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Lấy danh sách các hóa đơn có trạng thái "Chưa thanh toán"
                var chuaThanhToanHoaDons = await _context.HoaDons
                    .Where(hd => hd.TrangThai == "Chưa thanh toán")
                    .ToListAsync();

                // Kiểm tra và cập nhật trạng thái đồng hồ cho các hóa đơn có trạng thái "Chưa thanh toán"
                await _thongBaoService.KiemTraVaCapNhatTrangThaiDongHo(chuaThanhToanHoaDons);

                // Đợi 1 ngày (24 giờ) trước khi kiểm tra lại
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }


}
