using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models.Words;

[Table("languages",Schema = "words")]
public class Language : AuditableModelBase<long>
{
    [Column("name")] public string Name { get; set; }
    [Column("code")] public string Code { get; set; }
    [Column("is_default")] public bool IsDefault { get; set; } = false;
}