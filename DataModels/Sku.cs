using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("SKU")]
public partial class Sku
{
    [Key]
    public int Id { get; set; }

    public int? FabricId { get; set; }

    [StringLength(10)]
    public string? BlindType { get; set; }

    public int? Price { get; set; }
}
