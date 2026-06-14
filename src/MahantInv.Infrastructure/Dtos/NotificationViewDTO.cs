using MahantInv.Infrastructure.Entities;
using System.Text.RegularExpressions;

namespace MahantInv.Infrastructure.Dtos
{
    public class NotificationViewDTO : Notification
    {
        public string ProductName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Message))
                    return null;

                var match = Regex.Match(Message, @"^(.*?),");
                return match.Success ? match.Groups[1].Value.Trim() : null;
            }
        }

        public int? ReorderLevel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Message))
                    return null;

                var match = Regex.Match(Message, @"Reorder Level\s*:\s*(\d+)", RegexOptions.IgnoreCase);
                return match.Success ? int.Parse(match.Groups[1].Value) : null;
            }
        }

        public int? CurrentStock
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Message))
                    return null;

                var match = Regex.Match(Message, @"Current Stock\s*:\s*(\d+)", RegexOptions.IgnoreCase);
                return match.Success ? int.Parse(match.Groups[1].Value) : null;
            }
        }
    }

}
