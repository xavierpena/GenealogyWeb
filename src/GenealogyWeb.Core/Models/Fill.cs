using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Fill
    {
        public int id { get; set; }
        public int matrimoni_id { get; set; }
        public int persona_id { get; set; }
        public string observacions { get; set; }
    }
}
