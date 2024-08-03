﻿using System.ComponentModel.DataAnnotations.Schema;
using Entitys.Models;

namespace Entity.Models;

[Table("structures", Schema = "auth")]
public class Structure : AuditableModelBase<long>
{
    [Column("name")] public MultiLanguageField Name { get; set; }
    public virtual ICollection<StructurePermission> StructurePermissions { get; set; }
    [Column("is_default")] public bool IsDefault { get; set; }
}