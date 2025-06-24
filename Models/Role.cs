using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("roles")]
public class Role
{
    [Key]
    [Column("roleid")]
    public int RoleId { get; set; }

    [Column("rolename")]
    public required string Name { get; set; }

    //public List<RolePermission> Permissions { get; set; } = new();
}
