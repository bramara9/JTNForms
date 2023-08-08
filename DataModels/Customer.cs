using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

[Table("Customer")]
public partial class Customer
{
    [StringLength(20)]
    public string FirstName { get; set; } = null!;

    [StringLength(20)]
    public string? LastName { get; set; }

    [StringLength(40)]
    public string? EmailAddress { get; set; }

    [StringLength(12)]
    public string? PhoneNumber { get; set; }

    [StringLength(40)]
    public string? Address { get; set; }

    [StringLength(10)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? CustomerStatus { get; set; }

    [Key]
    public int Id { get; set; }

    [StringLength(10)]
    public string? Zip { get; set; }

    [StringLength(40)]
    public string? Community { get; set; }

    [StringLength(40)]
    public string? Address2 { get; set; }

    [Column("IsInchOrMM")]
    public bool? IsInchOrMm { get; set; }
}
