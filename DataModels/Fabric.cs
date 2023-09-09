using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("Fabric")]
public partial class Fabric
{
    [StringLength(50)]
    public string CatalogName { get; set; } = null!;

    [StringLength(50)]
    public string FabricName { get; set; } = null!;

    public byte[]? Image { get; set; }

    [StringLength(50)]
    public string? FabricType { get; set; }

    [Key]
    public int Id { get; set; }

    [StringLength(250)]
    public string? FileName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? FabricCode { get; set; }
}
