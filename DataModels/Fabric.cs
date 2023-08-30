using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Keyless]
[Table("Fabric")]
public partial class Fabric
{
    [StringLength(50)]
    public string CatalogName { get; set; } = null!;

    [StringLength(50)]
    public string FabricName { get; set; } = null!;

    [MaxLength(1000)]
    public byte[]? Image { get; set; }

    public int Price { get; set; }
}
