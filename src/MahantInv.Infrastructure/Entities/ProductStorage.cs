using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Keyless]
public partial class ProductStorage
{
    public int ProductId { get; set; }

    public int StorageId { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }

    [ForeignKey("StorageId")]
    public virtual Storage Storage { get; set; }
}
