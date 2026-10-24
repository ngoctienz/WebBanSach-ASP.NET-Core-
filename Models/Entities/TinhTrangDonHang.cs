using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("TinhTrangDonHang")]
    public class TinhTrangDonHang
    {
        [Key]
        public string MaTinhTrang { get; set; } = string.Empty;

        public string? TenTinhTrang { get; set; }

        public int Ordering { get; set; }
    }
}
