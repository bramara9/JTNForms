using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("ReorderWindow")]
public partial class ReorderWindow
{
    [Key]
    public int Id { get; set; }

    public int WindowId { get; set; }

    [StringLength(100)]
    public string? ControlType { get; set; }

    [StringLength(200)]
    public string? Option { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [StringLength(50)]
    public string? RoomName { get; set; }

    [StringLength(50)]
    public string? FabricName { get; set; }

    [StringLength(20)]
    public string? BlindType { get; set; }

    public int CustomerId { get; set; }

    public bool? Is2In1 { get; set; }

    public bool? IsNoValance { get; set; }

    public bool? IsNeedExtension { get; set; }

    public int? NoOfPanels { get; set; }

    [StringLength(10)]
    public string? StackType { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? OrderedHeight { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? OrderedWidth { get; set; }

    [ForeignKey("WindowId")]
    [InverseProperty("ReorderWindows")]
    public virtual Window Window { get; set; } = null!;
}
