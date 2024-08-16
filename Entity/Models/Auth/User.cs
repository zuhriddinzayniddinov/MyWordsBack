using System.ComponentModel.DataAnnotations.Schema;
using Entity.Enum;

namespace Entity.Models.Auth;

[Table("users", Schema = "auth")]
public class User : AuditableModelBase<long>
{
    [Column("firstname")] public string FirstName { get; set; }
    [Column("lastname")] public string LastName { get; set; }
    [Column("native_language")] public Language NativeLanguage { get; set; }
    [NotMapped] public virtual IEnumerable<SignMethod> SignMethods { get; set; }

    [Column("structure_id")]
    [ForeignKey("Structure")]
    public long? StructureId { get; set; }
    public virtual Structure? Structure { get; set; }
}