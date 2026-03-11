using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Categories
    {    
        public int cat_id { get; set; }
        [Required]
        public string? category_name { get; set; }
        public string? status { get; set; }
        public string? description { get; set; }
        public DateTime? date_created { get; set; }
    }
}
