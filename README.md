# WebBanSach — ASP.NET Core MVC

Trang web bán sách **Nhà Giả Kim** (Paulo Coelho): giới thiệu sách, tác giả, đọc thử, đánh giá và đặt hàng trực tuyến (COD / chuyển khoản ngân hàng).

- Framework: **ASP.NET Core 10 MVC**
- ORM: **Entity Framework Core 10** (SQL Server)
- QR: **QRCoder + SkiaSharp**

## Công nghệ sử dụng

| Lớp | Công cụ |
|---|---|
| Frontend | HTML, Tailwind CSS v4 (browser), JavaScript thuần, jQuery, Slick Carousel, StPageFlip |
| Backend | ASP.NET Core MVC, EF Core, FluentValidation |
| QR code | QRCoder, SkiaSharp |
| Database | SQL Server |

## Cấu trúc dự án

```
Models/
  Entities/      ThongTinSach, TacGia, DanhGia, DonDatHang, TinhTrangDonHang, TaiKhoan, CaiDat
  DTOs/          DonDatHangRequest, DonDatHangResponse, ThongTinThanhToanResponse, Responses<T>
  EF/            ApplicationDbContext
Services/        IBookServices, BookServices
Validators/      DonDatHangRequestValidator
Helpers/         Utils (VietQR EMV, vẽ QR bằng QRCoder + SkiaSharp)
Views/           Home/Index.cshtml, Shared/_Layout.cshtml
```

## Mô hình dữ liệu

- **ThongTinSach** (UID): tên, thể loại, tiêu đề, giá gốc, giá bán, ảnh, link YouTube, tác giả.
- **TacGia** (MaTacGia): tên, tiểu sử (HTML), ảnh đại diện.
- **DanhGia** (UID): người đánh giá, số sao, nội dung, avatar.
- **DonDatHang** (UID): mã đơn, khách hàng, SĐT, địa chỉ, số lượng, tổng tiền, phương thức thanh toán, trạng thái, thời gian đặt, ghi chú.
- **TinhTrangDonHang** (MaTinhTrang): mã + tên trạng thái (ví dụ: `CHO_XAC_NHAN` – Chờ xác nhận).
- **TaiKhoan**, **CaiDat**: quản trị.

## Chạy dự án

```bash
# Cấu hình connection string (appsettings.Development.json)
ConnectionStrings:
  WebBanSachCST: "Server=localhost;Database=WebBanSach;User Id=...;Password=...;TrustServerCertificate=True;"

# Cập nhật Database từ Migration
dotnet ef database update

# Chạy
dotnet run
```

Truy cập `http://localhost:5031`.

> Lưu ý: `appsettings.json` chỉ chứa Logging. Connection string được đặt trong `appsettings.Development.json`.

## Migration

- `20261017011213_init_database` — tạo 7 bảng.
- `20261017012504_SeedData` — seeding tác giả, sách, đánh giá.

Seed dữ liệu mặc định:
- Tác giả: Paulo Coelho (`TG01`).
- Sách UID 1: **Nhà Giả Kim**, giá bán 79.000đ (gốc 109.000đ).
- 6 đánh giá khách hàng.

## Luồng đặt hàng

1. Người dùng nhấn nút **Mua ngay** → mở modal `#order-dialog`.
2. `order.js` validate client-side (họ tên, SĐT, địa chỉ, số lượng, phương thức thanh toán).
3. Gửi POST JSON (`application/json`) tới `/DatHang` (cũng khớp `/Home/DatHang`), kèm CSRF token.
4. `HomeController.DatHang`:
   - Validate bằng **FluentValidation** → trả về 400 nếu lỗi.
   - Gọi `BookServices.DatHangAsync`.
5. `BookServices.DatHangAsync`:
   - Tính `tongTien = GiaBan × SoLuong`.
   - Tạo mã đơn: `DH` + `yyMMddHHmmss` + 3 chữ số ngẫu nhiên.
   - Lấy trạng thái mặc định `CHO_XAC_NHAN` từ `TinhTrangDonHang` (tự tạo nếu bảng chưa có).
   - Lưu đơn vào DB.
   - Trả response: `Tatus`, `message`, `ThanhCong`, `ThongBao`, thông tin đơn + **thông tin chuyển khoản** (MBBank, số tài khoản, nội dung chuyển khoản, VietQR).
6. Frontend hiển thị kết quả: trạng thái đơn, tổng tiền, khối chuyển khoản hoặc QR code.

## Các endpoint

| Method | Route | Mô tả |
|---|---|---|
| GET | `/` | Trang chủ (`Home/Index`) |
| POST | `/DatHang`, `/Home/DatHang` | Đặt hàng (JSON) |

## Ghi chú

- **QR code**: có 2 cách — `BookServices.DatHangAsync` tạo URL trực tiếp từ **VietQR API** (`img.vietqr.io`); `Helpers.Utils` cung cấp sinh mã VietQR EMV + checksum CRC16 và vẽ ảnh QR bằng QRCoder + SkiaSharp (tùy chọn logo, màu).
- **Modal**: một `<dialog id="order-dialog">` duy nhất trong `_Layout.cshtml`; mọi nút `data-open-order` đều mở cùng một modal.
- **Sticky CTA**: xuất hiện trên mobile khi nút CTA chính rời khỏi màn hình.
- **Ngôn ngữ**: toàn bộ message lỗi và UI bằng tiếng Việt.
- **Anti-forgery**: CSRF token bắt buộc cho POST, header `RequestVerificationToken`.

## TODO

- [ ] Chuyển connection string vào `appsettings.json` hoặc biến môi trường.
- [ ] Build Tailwind CSS cho production thay vì CDN.
- [ ] Thêm trang quản trị / xem danh sách đơn hàng.
- [ ] Dùng `Utils.GenerateQRCodeImage` thay URL VietQR API.
- [ ] Thêm xác thực và quản lý tài khoản.
