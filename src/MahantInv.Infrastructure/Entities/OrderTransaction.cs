using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Entities
{
    [Table("OrderTransactions")]
    public class OrderTransaction : BaseEntity, IAggregateRoot
    {
        public int OrderId { get; set; }
        public int PartyId { get; set; }
        public string PaymentTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("OrderId")]
        [InverseProperty("OrderTransactions")]
        public virtual Order Order { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("PartyId")]
        [InverseProperty("OrderTransactions")]
        public virtual Party Party { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("PaymentTypeId")]
        [InverseProperty("OrderTransactions")]
        public virtual PaymentType PaymentType { get; set; }
    }
}
