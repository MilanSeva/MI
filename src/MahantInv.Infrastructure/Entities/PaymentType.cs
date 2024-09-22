using MahantInv.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("PaymentTypes")]
    public class PaymentType : IAggregateRoot
    {
        [Dapper.Contrib.Extensions.ExplicitKey]
        public string Id { get; set; }
        public string Title { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("PaymentType")]
        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();
    }
}
