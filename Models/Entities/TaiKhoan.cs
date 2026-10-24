using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { get; set; }

        public string? Email { get; set; }

        public string? MatKhau { get; set; }

        public string? HoTen { get; set; }

        public string? Role { get; set; }
    }
}
