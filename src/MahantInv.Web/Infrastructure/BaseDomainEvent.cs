using MediatR;
using System;

namespace MahantInv.Web.Infrastructure
{
    public abstract class BaseDomainEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}