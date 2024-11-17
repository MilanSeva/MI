using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Dtos.Product
{
    public class ChangeProductImageRequest
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
    }
}
