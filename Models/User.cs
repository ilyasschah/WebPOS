using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("userid")]
        public int UserId { get; set; }

        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; } = "";

        [Required]
        [Column("passwordhash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = "";

        [Column("roleid")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [Column("businessid")]
        public int BusinessId { get; set; }
    }
}
