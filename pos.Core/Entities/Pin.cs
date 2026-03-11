using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Core.Entities
{
    public class Pin
    {
        public int id { get; set; }
        [Required]
        public int pin_code { get; set; }
        [Required]
        public string? first_name { get; set; }
        [Required]
        public string? middle_name { get; set; }
        [Required]
        public string? last_name { get; set; }
        [Required]
        public string? position { get; set; }
        [Required]
        public string? status { get; set; }
    }
}
