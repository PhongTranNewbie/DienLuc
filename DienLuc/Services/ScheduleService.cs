using DienLuc.Models;

namespace DienLuc.Services
{
	public class ScheduleService : BackgroundService
	{
		private readonly ILogger<ScheduleService> _logger;
		private readonly IServiceScopeFactory _scopeFactory;

		public ScheduleService(ILogger<ScheduleService> logger, IServiceScopeFactory scopeFactory)
		{
			_logger = logger;
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var scope = _scopeFactory.CreateScope())
				{
					var dbContext = scope.ServiceProvider.GetRequiredService<DienLucContext>();
					var activeMeters = dbContext.DongHoDiens.Where(m => m.TrangThai == "Đang hoạt động").ToList();

					foreach (var meter in activeMeters)
					{
						meter.TrangThai = "Đã tạm dừng hoạt động";
						dbContext.Update(meter);
						await dbContext.SaveChangesAsync();

						// Tạo và gửi thông báo cho khách hàng
						var notification = new ThongBao
						{
							NoiDung = $"Đồng hồ điện {meter.MaDh} đã tạm dừng hoạt động."
						};

						// Gửi thông báo cho khách hàng
						// Code gửi thông báo ở đây
					}
				}

				await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Lập lịch chạy mỗi ngày
			}
		}
	}

}
