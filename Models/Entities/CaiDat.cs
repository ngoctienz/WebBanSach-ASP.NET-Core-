using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Entities
{
    [Table("CaiDat")]
    public class CaiDat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { get; set; }

        public string? SettingKey { get; set; }

        public string? SettingContent { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
