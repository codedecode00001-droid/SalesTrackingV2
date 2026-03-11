using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Dashboard
    {      
        [Required]

        public string? order_id { get; set; }
        public DateTime? datetime { get; set; }
        public string? users { get; set; }
        public decimal? total_amount {  get; set; }

        public string? category_name { get; set; }
        public string? product_name { get; set; }
        public int? stock { get; set; }
        public DateTime? date_expired { get; set; }

        public int? total_product { get; set; }
        public int? total_category { get; set; }
        public int? no_sales { get; set; }
        public decimal? total_revenue { get; set; }


    }
}
