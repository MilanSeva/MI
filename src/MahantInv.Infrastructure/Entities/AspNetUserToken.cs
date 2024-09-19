﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[PrimaryKey("UserId", "LoginProvider", "Name")]
public partial class AspNetUserToken
{
    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string UserId { get; set; }

    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string LoginProvider { get; set; }

    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string Name { get; set; }

    public string Value { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserTokens")]
    public virtual AspNetUser User { get; set; }
}
