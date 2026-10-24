using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { get; set; }

        public string? HoTen { get; set; }

        public int SoSao { get; set; }

        public string? NoiDungDanhGia { get; set; }

        public string? AnhDaiDien { get; set; }
    }
}
