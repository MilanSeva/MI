using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Dtos
{
    public class ValidationError
    {
        public bool Success
        {
            get
            {
                return string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }
        public string? Key { get; set; }
        public string? ErrorMessage { get; set; }
        public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    }
    public enum ValidationSeverity
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }
}
