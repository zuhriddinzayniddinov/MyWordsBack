using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models.Auth;

[Table("users", Schema = "auth")]
public sealed class User : AuditableModelBase<long>
{
    [Column("firstname")] public string FirstName { get; set; }
    [Column("lastname")] public string LastName { get; set; }
    [NotMapped] public IEnumerable<SignMethod> SignMethods { get; set; }

    [Column("structure_id")]
    [ForeignKey("Structure")]
    public long? StructureId { get; set; }

    public Structure? Structure { get; set; }
}