using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanSach.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DanhGia",
                columns: new[] { "Uid", "AnhDaiDien", "HoTen", "NoiDungDanhGia", "SoSao" },
                values: new object[,]
                {
                    { 1, "https://randomuser.me/api/portraits/women/12.jpg", "Minh Anh", "Bìa tím rất đẹp, in chữ vàng nổi bật. Câu chuyện nhẹ nhàng nhưng đọc xong cứ nghĩ mãi về “huyền thoại cá nhân” của chính mình.", 5 },
                    { 2, "https://randomuser.me/api/portraits/men/32.jpg", "Quốc Bảo", "Mua làm quà sinh nhật cho em gái, sách được gói cẩn thận. Đây là cuốn mình đọc lại nhiều lần nhất.", 5 },
                    { 3, "https://randomuser.me/api/portraits/women/45.jpg", "Thu Hà", "Văn phong giản dị, dễ đọc. Có vài đoạn mang tính ẩn dụ cần đọc chậm để thấm. Rất đáng để sở hữu.", 4 },
                    { 4, "https://randomuser.me/api/portraits/men/68.jpg", "Hoàng Nam", "Đọc trong một buổi tối mà thấy được tiếp thêm năng lượng. Phù hợp cho những ai đang phân vân trước lựa chọn lớn.", 5 },
                    { 5, "https://randomuser.me/api/portraits/women/24.jpg", "Lan Phương", "Giấy đẹp, chữ dễ đọc. Mình thích nhất đoạn Santiago ở sa mạc cùng nhà giả kim.", 4 },
                    { 6, "https://randomuser.me/api/portraits/men/86.jpg", "Đức Huy", "Cuốn sách nhỏ nhưng chứa nhiều điều lớn. Mỗi lần đọc lại ở một giai đoạn khác nhau lại thấy một ý nghĩa mới.", 5 }
                });

            migrationBuilder.InsertData(
                table: "TacGia",
                columns: new[] { "MaTacGia", "AnhDaiDien", "GioiThieu", "HoTenTacGia" },
                values: new object[] { "TG01", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJz5a4S33Jd6Zetrh5OCxcJ7YCrPKhntb1sb21mn8HU0ULyTAjsRA71Y0&s=10", "<p>Paulo Coelho sinh năm 1947 tại Rio de Janeiro, Brazil. Trước khi trở thành nhà văn, ông từng viết lời bài hát, làm việc trong lĩnh vực sân khấu và báo chí. Chuyến hành hương trên con đường Santiago de Compostela năm 1986 là bước ngoặt đưa ông đến với sự nghiệp văn chương.</p>\n<p>Văn phong của Coelho giản dị, giàu tính biểu tượng và chiêm nghiệm, thường xoay quanh hành trình tìm kiếm bản thân, niềm tin và ý nghĩa cuộc sống. <em>Nhà Giả Kim</em> (<em>O Alquimista</em>, 1988) trở thành tác phẩm nổi tiếng nhất của ông, được dịch ra rất nhiều ngôn ngữ và truyền cảm hứng cho nhiều thế hệ độc giả.</p>\n<p>Các tác phẩm tiêu biểu khác: <em>Hành hương</em>, <em>Bên dòng sông Piedra tôi ngồi khóc</em>, <em>Veronika quyết chết</em>, <em>Mười một phút</em>.</p>", "Paulo Coelho" });

            migrationBuilder.InsertData(
                table: "ThongTinSach",
                columns: new[] { "Uid", "AnhSach", "FileDocThu", "GiaBan", "GiaGoc", "LinkYouTube", "MaTacGia", "TenSach", "TheLoai", "TieuDeChinh", "TieuDePhu" },
                values: new object[] { 1, "images/sach-nha-gia-kim.png", null, 79000m, 109000m, "https://www.youtube.com/watch?v=SpYfZOEOyG4", "TG01", "Nhà Giả Kim", "Tiểu thuyết", "Một hành trình đầy cảm hứng về giấc mơ, lòng dũng cảm và việc lắng nghe tiếng gọi của trái tim.", "Theo chân Santiago trên hành trình đi tìm kho báu và khám phá ý nghĩa của việc theo đuổi ước mơ." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DanhGia",
                keyColumn: "Uid",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TacGia",
                keyColumn: "MaTacGia",
                keyValue: "TG01");

            migrationBuilder.DeleteData(
                table: "ThongTinSach",
                keyColumn: "Uid",
                keyValue: 1);
        }
    }
}
