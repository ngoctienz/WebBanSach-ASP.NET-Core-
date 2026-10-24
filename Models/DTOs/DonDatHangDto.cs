using System;

namespace WebBanSach.Models.DTOs
{
    // Model Request gửi lên từ Client khi đặt hàng (các field tiếng Việt đồng bộ với Entity DonDatHang)
    public class DonDatHangRequest
    {
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public string? DiaChi { get; set; }
        public int SoLuong { get; set; } = 1;
        public string? MaHinhThucThanhToan { get; set; } = "cod";
        public string? GhiChu { get; set; }

        // Alias tương thích ngược nếu client gửi tên tiếng Anh
        public string? FullName { get => TenKhachHang; set => TenKhachHang = value; }
        public string? Phone { get => SDT; set => SDT = value; }
        public string? Address { get => DiaChi; set => DiaChi = value; }
        public int Quantity { get => SoLuong; set => SoLuong = value; }
        public string? PaymentMethod { get => MaHinhThucThanhToan; set => MaHinhThucThanhToan = value; }
        public string? Note { get => GhiChu; set => GhiChu = value; }
    }

    // Model Response trả về cho Client sau khi đặt hàng
    public class DonDatHangResponse
    {
        // Các trường response chung
        public int Tatus { get; set; } = 200;
        public string? message { get; set; }
        public object? Eror { get; set; }

        public bool ThanhCong { get; set; } = true;
        public string? ThongBao { get; set; }
        public string? MaDonHang { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public string? DiaChi { get; set; }
        public int SoLuong { get; set; }
        public decimal TongTien { get; set; }
        public string? MaHinhThucThanhToan { get; set; }
        public string? MaTinhTrang { get; set; }
        public string? TenTinhTrang { get; set; }
        public DateTime ThoiGianDat { get; set; }
        public string? GhiChu { get; set; }
        public ThongTinThanhToanResponse? ThongTinThanhToan { get; set; }

        // Alias cho JS (sử dụng return, không dùng arrow function)
        public string? OrderId
        {
            get { return MaDonHang; }
        }

        public decimal Total
        {
            get { return TongTien; }
        }

        public string? Status
        {
            get { return MaTinhTrang; }
        }

        public ThongTinThanhToanResponse? Payment
        {
            get { return ThongTinThanhToan; }
        }
    }

    public class ThongTinThanhToanResponse
    {
        public string? TenNganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTaiKhoan { get; set; }
        public decimal SoTien { get; set; }
        public string? NoiDungChuyenKhoan { get; set; }
        public string? LinkQrCode { get; set; }

        // Alias cho JS (sử dụng return, không dùng arrow function)
        public string? BankName
        {
            get { return TenNganHang; }
        }

        public string? AccountNumber
        {
            get { return SoTaiKhoan; }
        }

        public string? AccountName
        {
            get { return TenChuTaiKhoan; }
        }

        public string? TransferContent
        {
            get { return NoiDungChuyenKhoan; }
        }

        public string? QrImageUrl
        {
            get { return LinkQrCode; }
        }
    }

    // Để tương thích nếu có code cũ tham chiếu
    public class OrderRequestDto : DonDatHangRequest { }
}
