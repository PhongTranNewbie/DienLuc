using DienLuc.Models;
using DienLuc.Services;
using Microsoft.EntityFrameworkCore;

public class ThongBaoService : IThongBaoService
{
    private readonly DienLucContext _context;

    public ThongBaoService(DienLucContext context)
    {
        _context = context;
    }

    public async Task<int> DemSoLanThongBao(string maHoaDon)
    {
        return await _context.ThongBaos.CountAsync(tb => tb.MaHd == maHoaDon);
    }

    public async Task KiemTraVaCapNhatTrangThaiDongHo(List<HoaDon> hoaDons)
    {
        foreach (var hoaDon in hoaDons)
        {
            string maHoaDon = hoaDon.MaHd;

            int soLanThongBao = await DemSoLanThongBao(maHoaDon);

            if (soLanThongBao >= 3)
            {
                var hoaDonEntity = await _context.HoaDons
                    .Where(x => x.MaHd == maHoaDon)
                    .Include(x => x.MaChiTietSddienNavigation)
                    .ThenInclude(x => x.MaDhNavigation)
                    .FirstOrDefaultAsync();

                if (hoaDonEntity != null)
                {
                    var dongHo = _context.DongHoDiens.FirstOrDefault(x => x.MaDh == hoaDonEntity.MaChiTietSddienNavigation.MaDh);

                    if (dongHo != null)
                    {
                        // Lấy ngày hết hạn từ thông báo cuối cùng
                        var ngayHetHan = await _context.ThongBaos
                            .Where(x => x.MaHd == maHoaDon)
                            .OrderByDescending(x => x.NgayThongBao)
                            .Select(x => x.NgayHetHan)
                            .FirstOrDefaultAsync();

                        // Tạo đối tượng DateOnly đại diện cho ngày hôm nay
                        var today = DateOnly.FromDateTime(DateTime.UtcNow);

                        if (today > ngayHetHan)
                        {
                            // Cập nhật trạng thái đồng hồ thành "Đã tạm dừng hoạt động"
                            dongHo.TrangThai = "Đã tạm dừng hoạt động";
                            _context.Update(dongHo);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }


}

