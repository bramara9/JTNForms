using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

public partial class Issue
{
    [Key]
    [Column(TypeName = "numeric(18, 0)")]
    public decimal Id { get; set; }

    public int WindowId { get; set; }

    public int CustomerId { get; set; }

    public string? Notes { get; set; }

    public string? Description { get; set; }

    [StringLength(10)]
    public string? Resolution { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Issues")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("WindowId")]
    [InverseProperty("Issues")]
    public virtual Window Window { get; set; } = null!;
}
