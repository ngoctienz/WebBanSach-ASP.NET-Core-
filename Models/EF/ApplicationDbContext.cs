using Microsoft.EntityFrameworkCore;
using WebBanSach.Models.Entities;

namespace WebBanSach.Models.EF
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ThongTinSach> ThongTinSachs { get; set; }
        public DbSet<DonDatHang> DonDatHangs { get; set; }
        public DbSet<TinhTrangDonHang> TinhTrangDonHangs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<CaiDat> CaiDats { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<TacGia> TacGias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Data Seeding: Tác giả (Giới thiệu lưu dưới dạng HTML)
            modelBuilder.Entity<TacGia>().HasData(
                new TacGia
                {
                    MaTacGia = "TG01",
                    HoTenTacGia = "Paulo Coelho",
                    GioiThieu = "<p>Paulo Coelho sinh năm 1947 tại Rio de Janeiro, Brazil. Trước khi trở thành nhà văn, ông từng viết lời bài hát, làm việc trong lĩnh vực sân khấu và báo chí. Chuyến hành hương trên con đường Santiago de Compostela năm 1986 là bước ngoặt đưa ông đến với sự nghiệp văn chương.</p>\n<p>Văn phong của Coelho giản dị, giàu tính biểu tượng và chiêm nghiệm, thường xoay quanh hành trình tìm kiếm bản thân, niềm tin và ý nghĩa cuộc sống. <em>Nhà Giả Kim</em> (<em>O Alquimista</em>, 1988) trở thành tác phẩm nổi tiếng nhất của ông, được dịch ra rất nhiều ngôn ngữ và truyền cảm hứng cho nhiều thế hệ độc giả.</p>\n<p>Các tác phẩm tiêu biểu khác: <em>Hành hương</em>, <em>Bên dòng sông Piedra tôi ngồi khóc</em>, <em>Veronika quyết chết</em>, <em>Mười một phút</em>.</p>",
                    AnhDaiDien = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJz5a4S33Jd6Zetrh5OCxcJ7YCrPKhntb1sb21mn8HU0ULyTAjsRA71Y0&s=10"
                }
            );

            // 2. Data Seeding: Thông tin sách Nhà Giả Kim
            modelBuilder.Entity<ThongTinSach>().HasData(
                new ThongTinSach
                {
                    Uid = 1,
                    TenSach = "Nhà Giả Kim",
                    TheLoai = "Tiểu thuyết",
                    TieuDeChinh = "Một hành trình đầy cảm hứng về giấc mơ, lòng dũng cảm và việc lắng nghe tiếng gọi của trái tim.",
                    TieuDePhu = "Theo chân Santiago trên hành trình đi tìm kho báu và khám phá ý nghĩa của việc theo đuổi ước mơ.",
                    GiaGoc = 109000m,
                    GiaBan = 79000m,
                    AnhSach = "images/sach-nha-gia-kim.png",
                    LinkYouTube = "https://www.youtube.com/watch?v=SpYfZOEOyG4",
                    MaTacGia = "TG01",
                    FileDocThu = null
                }
            );

            // 3. Data Seeding: Toàn bộ Feedback / Đánh giá từ khách hàng
            modelBuilder.Entity<DanhGia>().HasData(
                new DanhGia
                {
                    Uid = 1,
                    HoTen = "Minh Anh",
                    SoSao = 5,
                    NoiDungDanhGia = "Bìa tím rất đẹp, in chữ vàng nổi bật. Câu chuyện nhẹ nhàng nhưng đọc xong cứ nghĩ mãi về “huyền thoại cá nhân” của chính mình.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/women/12.jpg"
                },
                new DanhGia
                {
                    Uid = 2,
                    HoTen = "Quốc Bảo",
                    SoSao = 5,
                    NoiDungDanhGia = "Mua làm quà sinh nhật cho em gái, sách được gói cẩn thận. Đây là cuốn mình đọc lại nhiều lần nhất.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/men/32.jpg"
                },
                new DanhGia
                {
                    Uid = 3,
                    HoTen = "Thu Hà",
                    SoSao = 4,
                    NoiDungDanhGia = "Văn phong giản dị, dễ đọc. Có vài đoạn mang tính ẩn dụ cần đọc chậm để thấm. Rất đáng để sở hữu.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/women/45.jpg"
                },
                new DanhGia
                {
                    Uid = 4,
                    HoTen = "Hoàng Nam",
                    SoSao = 5,
                    NoiDungDanhGia = "Đọc trong một buổi tối mà thấy được tiếp thêm năng lượng. Phù hợp cho những ai đang phân vân trước lựa chọn lớn.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/men/68.jpg"
                },
                new DanhGia
                {
                    Uid = 5,
                    HoTen = "Lan Phương",
                    SoSao = 4,
                    NoiDungDanhGia = "Giấy đẹp, chữ dễ đọc. Mình thích nhất đoạn Santiago ở sa mạc cùng nhà giả kim.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/women/24.jpg"
                },
                new DanhGia
                {
                    Uid = 6,
                    HoTen = "Đức Huy",
                    SoSao = 5,
                    NoiDungDanhGia = "Cuốn sách nhỏ nhưng chứa nhiều điều lớn. Mỗi lần đọc lại ở một giai đoạn khác nhau lại thấy một ý nghĩa mới.",
                    AnhDaiDien = "https://randomuser.me/api/portraits/men/86.jpg"
                }
            );
        }
    }
}