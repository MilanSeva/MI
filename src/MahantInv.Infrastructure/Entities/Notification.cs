using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "TEXT (100)")]
    public string Title { get; set; }

    [Required]
    [Column(TypeName = "TEXT (900)")]
    public string Message { get; set; }

    [Required]
    [Column(TypeName = "TEXT (10)")]
    public string Status { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime ModifiedAt { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime CreatedAt { get; set; }
}
