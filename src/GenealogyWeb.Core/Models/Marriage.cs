using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Marriage
    {
        public int? id { get; set; }
        [Display(Name = "Husband")]
        public int? husband_id { get; set; }
        [Display(Name = "Wife")]
        public int? wife_id { get; set; }
        [Display(Name = "Date (yyyy-MM-dd)")]        
        public string date { get; set; }
        [Display(Name = "Place")]
        public string place { get; set; }
        [Display(Name = "Comments")]
        public string comments { get; set; }

        // ===

        public override string ToString()
            => $"{id}; {date}; {place}; {comments};";
        
    }
}
