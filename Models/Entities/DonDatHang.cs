using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("DonDatHang")]
    public class DonDatHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { get; set; }

        public string? MaDonHang { get; set; }

        public int SoLuong { get; set; }

        public string? TenKhachHang { get; set; }

        public string? SDT { get; set; }

        public string? DiaChi { get; set; }

        public string? MaHinhThucThanhToan { get; set; }

        public string? MaTinhTrang { get; set; }

        public DateTime ThoiGianDat { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTien { get; set; }

        public string? GhiChu { get; set; }
    }
}
