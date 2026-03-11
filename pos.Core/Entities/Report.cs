using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Report
    {
        public string? orderTrans { get; set; }
        public string? product_name { get; set; }   // "P1, P2, P3"
        public string? price { get; set; }          // "100, 200, 300"
        public string? amount { get; set; }         // "1000, 2000, 3000"
        public int total_sales { get; set; }         // 6000
        public string? date { get; set; }
    }
}
