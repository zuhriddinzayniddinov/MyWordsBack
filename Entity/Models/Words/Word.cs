using System.ComponentModel.DataAnnotations.Schema;
using Entity.Models.Auth;

namespace Entity.Models.Words;

[Table("words",Schema = "words")]
public class Word : AuditableModelBase<long>
{
    [Column("text")] public string Text { get; set; }
    [Column("translation")] public string Translation { get; set; }
    [Column("user_id"),ForeignKey(nameof(User))] public long UserId { get; set; }
    public virtual User User { get; set; }
    [Column("group_id"),ForeignKey(nameof(Group))] public long GroupId { get; set; }
    public virtual Group Group { get; set; }
}