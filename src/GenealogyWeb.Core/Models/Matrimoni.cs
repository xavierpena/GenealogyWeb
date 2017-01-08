using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Matrimoni
    {
        public int id { get; set; }
        public int? home_id { get; set; }
        public int? dona_id { get; set; }
        public string data { get; set; }
        public string lloc { get; set; }
        public string observacions { get; set; }

        // ===

        public override string ToString()
            => $"{id}; {data}; {lloc}; {observacions};";

        public object GetSearchKey()
           => $"id:{id};home_id:{home_id};dona_id={dona_id}";
    }
}
