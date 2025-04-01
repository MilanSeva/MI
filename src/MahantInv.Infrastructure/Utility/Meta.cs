using System;

namespace MahantInv.Infrastructure.Utility
{
    public static class Meta
    {
        public class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string ProductView = "ProductView";
        }
        public static DateTime Now = DateTime.UtcNow;
        public class OrderStatusTypes
        {
            public const string Ordered = "Ordered";
            public const string Received = "Received";
            public const string Cancelled = "Cancelled";
        }
        public class PaymentStatuses
        {
            public const string Unpaid = "Unpaid";
            public const string PartiallyPaid = "Partially Paid";
            public const string Paid = "Paid";
        }
        public class PartyTypes
        {
            public const string Payer = "Payer";
            public const string Seller = "Seller";
            public const string Both = "Both";
        }
        public class NotificationStatusTypes
        {
            public const string Pending = "Pending";
            public const string Notified = "Notified";
            //public const string Marked = "Marked";
            public const string Read = "Read";
            //public const string Cancelled = "Cancelled";
        }
    }
}
