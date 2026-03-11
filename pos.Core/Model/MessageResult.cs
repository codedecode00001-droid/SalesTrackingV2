using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Model
{
    public class MessageResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int prodId { get; set; }
    }
}
