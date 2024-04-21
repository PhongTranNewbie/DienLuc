using DienLuc.Models;
using Microsoft.EntityFrameworkCore;

namespace DienLuc.Services
{
	public class MeterService
	{
		private readonly DienLucContext _context;

		public MeterService(DienLucContext context)
		{
			_context = context;
		}

		public async Task UpdateMeterStatusAsync(string maDh)
		{
			var meter = await _context.DongHoDiens.FirstOrDefaultAsync(m => m.MaDh == maDh);
			if (meter != null)
			{
				var hoaDonHienTai = await _context.HoaDons
					.Where(h => h.MaChiTietSddienNavigation != null && h.MaChiTietSddienNavigation.MaDh == maDh)
					.LastOrDefaultAsync();

				var hoaDonTruocDo = await _context.HoaDons
					.Where(h => h.MaChiTietSddienNavigation != null && h.MaChiTietSddienNavigation.MaDh == maDh)
					.OrderByDescending(h => h.NgayDenHan)
					.Skip(1)
					.FirstOrDefaultAsync();

				if (hoaDonHienTai != null && hoaDonTruocDo != null)
				{
					// Kiểm tra nếu cả hai hóa đơn đều chưa thanh toán và đã đến hạn
					if (hoaDonHienTai.TrangThai == "Chưa thanh toán" && hoaDonHienTai.NgayDenHan <= DateTime.Now &&
						hoaDonTruocDo.TrangThai == "Đã chuyển vào hóa đơn tiếp theo" && hoaDonTruocDo.NgayDenHan <= DateTime.Now)
					{
						// Cập nhật trạng thái của mã đồng hồ
						meter.TrangThai = "Đã tạm dừng hoạt động";
						_context.Update(meter);
						await _context.SaveChangesAsync();

						// Tạo và gửi thông báo cho khách hàng
						var notification = new ThongBao
						{
							NoiDung = $"Đồng hồ điện {meter.MaDh} đã tạm dừng hoạt động."
						};

						// Gửi thông báo cho khách hàng
						// Code gửi thông báo ở đây
					}
				}
			}
		}
	}
}
