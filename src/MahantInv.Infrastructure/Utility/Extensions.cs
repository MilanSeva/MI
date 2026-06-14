using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Utility
{
    public static class Extensions
    {
        public static string UTCToISTFormat(this DateTime dt)
        {
            return dt.AddMinutes(330).ToString("dd MMM yyyy hh:mm tt");
            //return TimeZoneInfo.ConvertTimeFromUtc(dt,
            //TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).ToString("dd MMM yy hh:mm:ss tt");
        }

        public static string? ToTitleCase(this string str)
        {
            return str.IsNullOrWhiteSpace() ? null : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
       
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }
        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        {
            return String.IsNullOrWhiteSpace(value);
        }
        public static string FormatAccountNumber(this string value)
        {
            return value.PadLeft(4, '0');
        }
        public static IEnumerable<string> QuotedSplit(this string s, string delim)
        {
            const char quote = '\'';

            var sb = new StringBuilder(s.Length);
            var counter = 0;
            while (counter < s.Length)
            {
                // if starts with delmiter if so read ahead to see if matches
                if (delim[0] == s[counter] &&
                    delim.SequenceEqual(ReadNext(s, counter, delim.Length)))
                {
                    yield return sb.ToString();
                    sb.Clear();
                    counter = counter + delim.Length; // Move the counter past the delimiter 
                }
                // if we hit a quote read until we hit another quote or end of string
                else if (s[counter] == quote)
                {
                    sb.Append(s[counter++]);
                    while (counter < s.Length && s[counter] != quote)
                    {
                        sb.Append(s[counter++]);
                    }
                    // if not end of string then we hit a quote add the quote
                    if (counter < s.Length)
                    {
                        sb.Append(s[counter++]);
                    }
                }
                else
                {
                    sb.Append(s[counter++]);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }
        private static IEnumerable<char> ReadNext(string str, int currentPosition, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (currentPosition + i >= str.Length)
                {
                    yield break;
                }
                else
                {
                    yield return str[currentPosition + i];
                }
            }
        }

    }
}
