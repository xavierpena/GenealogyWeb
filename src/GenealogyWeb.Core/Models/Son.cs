using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Son
    {        
        public int? id { get; set; }
        [Display(Name = "Marriage")]
        public int marriage_id { get; set; }
        [Display(Name = "Son")]
        public int person_id { get; set; }
        [Display(Name = "Comments")]
        public string comments { get; set; }

    }
}
