using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Inventory
    {
        public int? prod_id { get; set; }
        public int? cat_id { get; set; }
        public string? category_name { get; set; }
        public string? product_name { get; set; }
        public int? price { get; set; }
        public int? stock { get; set; }
        public string? date_expired { get; set; }
        public string? status { get; set; }
        public string? description { get; set; }
    }
}
