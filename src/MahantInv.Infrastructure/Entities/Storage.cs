using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class Storage
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (128)")]
    public string Name { get; set; }

    [Column(TypeName = "BOOL")]
    public bool Enabled { get; set; }
}
