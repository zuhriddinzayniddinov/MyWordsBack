using System.ComponentModel.DataAnnotations.Schema;
using Entity.Models.Auth;

namespace Entity.Models.Words;

[Table("groups",Schema = "words")]
public class Group : AuditableModelBase<long>
{
    [Column("name")] public string Name { get; set; }
    [Column("to_language_id"),ForeignKey(nameof(ToLanguage))] public long ToLanguageId { get; set; }
    public virtual Language ToLanguage { get; set; }
    [Column("from_language_id"),ForeignKey(nameof(FromLanguage))] public long FromLanguageId { get; set; }
    public virtual Language FromLanguage { get; set; }
    [Column("user_id"),ForeignKey(nameof(User))] public long UserId { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<Word> Words { get; set; }
}