using MahantInv.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Entities
{
    [Table("ProductExpiries")]
    public class ProductExpiry : BaseEntity
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Product Product { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Order Order { get; set; }

        public bool IsArchive { get; set; }

    }
}
