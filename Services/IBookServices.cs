using System.Collections.Generic;
using System.Threading.Tasks;
using WebBanSach.Models.DTOs;
using WebBanSach.Models.Entities;

namespace WebBanSach.Services
{
    public interface IBookServices
    {
        // Các phương thức lấy thông tin sách, tác giả, đánh giá
        Task<ThongTinSach?> GetBookAsync(int uid = 1);
        Task<TacGia?> GetAuthorAsync(string maTacGia);
        Task<List<DanhGia>> GetReviewsAsync();

        // Các phương thức xử lý đơn hàng chuyển toàn bộ về BookServices
        Task<DonDatHangResponse> DatHangAsync(DonDatHangRequest request);
        Task<DonDatHang> CreateOrderAsync(DonDatHang donHang);
        Task<DonDatHang?> GetOrderByIdAsync(string maDonHang);
        Task<string> GetDefaultOrderStatusAsync();
        Task<List<TinhTrangDonHang>> GetOrderStatusesAsync();
    }
}
