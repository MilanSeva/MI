using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("PaymentTypes")]
    public class PaymentType : IAggregateRoot
    {
        public string Id { get; set; }
        public string Title { get; set; }

        [InverseProperty("PaymentType")]
        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();
    }
}
