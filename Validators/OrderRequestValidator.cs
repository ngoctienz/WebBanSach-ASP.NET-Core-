using FluentValidation;
using WebBanSach.Models.DTOs;

namespace WebBanSach.Validators
{
    public class DonDatHangRequestValidator : AbstractValidator<DonDatHangRequest>
    {
        public DonDatHangRequestValidator()
        {
            RuleFor(x => x.TenKhachHang)
                .NotEmpty().WithMessage("Vui lòng nhập họ và tên khách hàng.")
                .Length(2, 100).WithMessage("Họ và tên khách hàng phải từ 2 đến 100 ký tự.");

            RuleFor(x => x.SDT)
                .NotEmpty().WithMessage("Vui lòng nhập số điện thoại.")
                .Matches(@"^(03|05|07|08|09)\d{8}$").WithMessage("Số điện thoại không đúng định dạng Việt Nam (VD: 0912 345 678).");

            RuleFor(x => x.DiaChi)
                .NotEmpty().WithMessage("Vui lòng nhập địa chỉ nhận hàng.")
                .MinimumLength(5).WithMessage("Địa chỉ nhận hàng quá ngắn, vui lòng ghi rõ số nhà, tên đường, phường/xã...");

            RuleFor(x => x.SoLuong)
                .GreaterThan(0).WithMessage("Số lượng mua tối thiểu là 1.")
                .LessThanOrEqualTo(10).WithMessage("Số lượng mua tối đa là 10 cuốn mỗi đơn.");

            RuleFor(x => x.MaHinhThucThanhToan)
                .NotEmpty().WithMessage("Vui lòng chọn hình thức thanh toán.")
                .Must(p => p == "cod" || p == "bank_transfer")
                .WithMessage("Hình thức thanh toán không hợp lệ (chỉ chấp nhận COD hoặc Chuyển khoản).");
        }
    }
}
