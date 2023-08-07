using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("Window")]
public partial class Window
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    public string? WindowName { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Width { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Height { get; set; }

    [StringLength(10)]
    public string? ControlType { get; set; }

    [StringLength(10)]
    public string? Option { get; set; }

    [StringLength(50)]
    public string? Notes { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TotalPrice { get; set; }

    [StringLength(40)]
    public string? RoomName { get; set; }

    public int? BasePrice { get; set; }

    [StringLength(20)]
    public string? FabricName { get; set; }

    [StringLength(20)]
    public string? BlindType { get; set; }

    public int CustomerId { get; set; }
}
