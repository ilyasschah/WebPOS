using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("rolepermissions")]
    public class RolePermission
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("roleid")]
        public int RoleId { get; set; }

        [Column("permission")]
        public string Permission { get; set; } = "";

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
