using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanSach.Models.DTOs;
using WebBanSach.Models.EF;
using WebBanSach.Models.Entities;

namespace WebBanSach.Services
{
    public class BookServices : IBookServices
    {
        private readonly ApplicationDbContext _context;

        public BookServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ThongTinSach?> GetBookAsync(int uid = 1)
        {
            return _context.ThongTinSachs.AsNoTracking().FirstOrDefaultAsync(x => x.Uid == uid);
        }

        public Task<TacGia?> GetAuthorAsync(string maTacGia)
        {
            return _context.TacGias.AsNoTracking().FirstOrDefaultAsync(x => x.MaTacGia == maTacGia);
        }

        public Task<List<DanhGia>> GetReviewsAsync()
        {
            return _context.DanhGias.AsNoTracking().OrderBy(x => x.Uid).ToListAsync();
        }

        public async Task<string> GetDefaultOrderStatusAsync()
        {
            var status = await _context.TinhTrangDonHangs
                .AsNoTracking()
                .OrderBy(x => x.Ordering)
                .FirstOrDefaultAsync();

            if (status != null)
            {
                return status.MaTinhTrang;
            }

            // Nếu bảng trong DB chưa có dữ liệu, tạo và lưu bản ghi mặc định vào DB
            var defaultStatus = new TinhTrangDonHang
            {
                MaTinhTrang = "CHO_XAC_NHAN",
                TenTinhTrang = "Chờ xác nhận",
                Ordering = 1
            };
            _context.TinhTrangDonHangs.Add(defaultStatus);
            await _context.SaveChangesAsync();
            return defaultStatus.MaTinhTrang;
        }

        public Task<List<TinhTrangDonHang>> GetOrderStatusesAsync()
        {
            return _context.TinhTrangDonHangs
                .AsNoTracking()
                .OrderBy(x => x.Ordering)
                .ToListAsync();
        }

        public Task<DonDatHang?> GetOrderByIdAsync(string maDonHang)
        {
            return _context.DonDatHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDonHang == maDonHang);
        }

        public async Task<DonDatHang> CreateOrderAsync(DonDatHang donHang)
        {
            _context.DonDatHangs.Add(donHang);
            await _context.SaveChangesAsync();
            return donHang;
        }

        public async Task<DonDatHangResponse> DatHangAsync(DonDatHangRequest request)
        {
            // 1. Lấy thông tin sách để tính tổng tiền
            var book = await GetBookAsync(1);
            decimal giaBan = book?.GiaBan ?? 79000m;
            decimal tongTien = giaBan * request.SoLuong;

            // 2. Tạo mã đơn hàng duy nhất
            var maDonHang = "DH" + DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(100, 999);

            // 3. Lấy mã tình trạng đơn hàng từ Database
            string maTinhTrang = await GetDefaultOrderStatusAsync();

            // 4. Khởi tạo thực thể DonDatHang
            var donHang = new DonDatHang
            {
                MaDonHang = maDonHang,
                TenKhachHang = request.TenKhachHang,
                SDT = request.SDT,
                DiaChi = request.DiaChi,
                SoLuong = request.SoLuong,
                TongTien = tongTien,
                MaHinhThucThanhToan = request.MaHinhThucThanhToan,
                MaTinhTrang = maTinhTrang,
                ThoiGianDat = DateTime.Now,
                GhiChu = request.GhiChu
            };

            // 5. Lưu vào Database
            await CreateOrderAsync(donHang);

            // 6. Trả về response model tiếng Việt đồng bộ
            var response = new DonDatHangResponse
            {
                Tatus = 200,
                message = "Đặt hàng thành công!",
                Eror = null,
                ThanhCong = true,
                ThongBao = "Đặt hàng thành công!",
                MaDonHang = donHang.MaDonHang,
                TenKhachHang = donHang.TenKhachHang,
                SDT = donHang.SDT,
                DiaChi = donHang.DiaChi,
                SoLuong = donHang.SoLuong,
                TongTien = donHang.TongTien,
                MaHinhThucThanhToan = donHang.MaHinhThucThanhToan,
                MaTinhTrang = donHang.MaTinhTrang,
                TenTinhTrang = "Chờ xác nhận",
                ThoiGianDat = donHang.ThoiGianDat,
                GhiChu = donHang.GhiChu,
                ThongTinThanhToan = new ThongTinThanhToanResponse
                {
                    TenNganHang = "Ngân hàng TMCP Quân đội (MBBank)",
                    SoTaiKhoan = "0987654321",
                    TenChuTaiKhoan = "NHA SACH GIA KIM",
                    SoTien = donHang.TongTien,
                    NoiDungChuyenKhoan = donHang.MaDonHang,
                    LinkQrCode = $"https://img.vietqr.io/image/MB-0987654321-compact2.png?amount={tongTien}&addInfo={donHang.MaDonHang}"
                }
            };

            return response;
        }
    }
}
