using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class ReturnOrder
    {
        public int order_no { get; set; }
        public string? order_id { get; set; }
        [Required]
        public int prod_id { get; set; }
        [Required]
        public int? cat_id { get; set; }
        public string? category_name { get; set; }
        public string? product_name { get; set; }
        public int? unit_id { get; set; }
        public string? unit_name { get; set; }
        public int? price { get; set; }
        public int? stock { get; set; }
        public int? qty { get; set; }
        public int? units { get; set; }
        public string? description { get; set; }
        public int? total_price { get; set; }
    }
}
