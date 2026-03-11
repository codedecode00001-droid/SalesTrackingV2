using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Order
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        [Required]
        public string? category_name { get; set; }
        public int? cat_id { get; set; }
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
