using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Matrimoni
    {
        public int? id { get; set; }
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
        
        public static Matrimoni MatrimoniFromArray(string[] values)
            => new Matrimoni
            {
                id = values[0] == null ? default(int?) : int.Parse(values[0]),
                home_id = values[1] == null ? default(int?) : int.Parse(values[1]),
                dona_id = values[2] == null ? default(int?) : int.Parse(values[2]),
                data = values[3],
                lloc = values[4],
                observacions = values[5]
            };
    }
}
