using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("TacGia")]
    public class TacGia
    {
        [Key]
        public string MaTacGia { get; set; } = string.Empty;

        public string? HoTenTacGia { get; set; }

        public string? GioiThieu { get; set; }

        public string? AnhDaiDien { get; set; }
    }
}
