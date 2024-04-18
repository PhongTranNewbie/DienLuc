using DienLuc.Models;
using System.ComponentModel.DataAnnotations;
namespace DienLuc.ViewModels
{
    public class TaiKhoanKhachHangViewModel
    {

        public string MaKh { get; set; }
        [Required(ErrorMessage = "Mã tài khoản là bắt buộc.")]
        public string MaTk { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Mk { get; set; }

        [Required(ErrorMessage = "CCCD là bắt buộc.")]
        public string Cccd { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc.")]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateOnly NgaySinh { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string SoDt { get; set; }
        [Required(ErrorMessage = "Mã thuế là bắt buộc.")]
        public string MaThue { get; set; }
		[Required(ErrorMessage = "Email là bắt buộc.")]
		public string Email { get; set; }
	}
}