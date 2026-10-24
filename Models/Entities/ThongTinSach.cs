using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("ThongTinSach")]
    public class ThongTinSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { get; set; }

        [Required]
        public string? TenSach { get; set; }

        public string? TheLoai { get; set; }

        public string? TieuDeChinh { get; set; }

        public string? TieuDePhu { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaGoc { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaBan { get; set; }

        public string? AnhSach { get; set; }

        public string? LinkYouTube { get; set; }

        public string? MaTacGia { get; set; }

        public string? FileDocThu { get; set; }
    }
}
