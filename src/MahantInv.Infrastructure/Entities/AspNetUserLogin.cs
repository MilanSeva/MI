using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[PrimaryKey("LoginProvider", "ProviderKey")]
[Index("UserId", Name = "IX_AspNetUserLogins_UserId")]
public partial class AspNetUserLogin
{
    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string LoginProvider { get; set; }

    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string ProviderKey { get; set; }

    public string ProviderDisplayName { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (450)")]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserLogins")]
    public virtual AspNetUser User { get; set; }
}
