using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("Room")]
public partial class Room
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    public string? RoomName { get; set; }

    [StringLength(20)]
    public string? FabricName { get; set; }

    [StringLength(20)]
    public string? BlindType { get; set; }

    public int? BasePrice { get; set; }

    [StringLength(50)]
    public string? Notes { get; set; }

    public int CustomerId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Rooms")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Room")]
    public virtual ICollection<Window> Windows { get; set; } = new List<Window>();
}
