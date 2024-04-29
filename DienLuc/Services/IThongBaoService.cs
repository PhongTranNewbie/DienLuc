using DienLuc.Models;

namespace DienLuc.Services
{
    public interface IThongBaoService
    {
        Task<int> DemSoLanThongBao(string maHoaDon);
        Task KiemTraVaCapNhatTrangThaiDongHo(List<HoaDon> hoaDons);
    }
}
