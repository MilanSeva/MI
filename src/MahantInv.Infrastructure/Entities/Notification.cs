using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("Notifications")]
    public class Notification : BaseEntity, IAggregateRoot
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime ModifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public double? Quantity { get; set; }
    }
}
