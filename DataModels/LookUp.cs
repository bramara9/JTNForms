using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Keyless]
[Table("LookUp")]
public partial class LookUp
{
    [StringLength(20)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }
}
