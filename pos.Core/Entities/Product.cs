using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Products
    {
        public int prod_id { get; set; }
        [Required]
        public string? category_name { get; set; }
        public int? cat_id { get; set; }
        public string? product_name { get; set; }
        public int? price { get; set; }
        public int? stock { get; set; }
        public string? date_expired { get; set; }
        public string? status { get; set; }
        public string? description { get; set; }
        public DateTime? date_created { get; set; }
    }
}
