using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Fill
    {
        public int? id { get; set; }
        public int matrimoni_id { get; set; }
        public int persona_id { get; set; }
        public string observacions { get; set; }

        public object GetSearchKey()
           => $"id:{id};matrimoni_id:{matrimoni_id};persona_id={persona_id}";

        public static Fill FillFromArray(string[] values)
            => new Fill
            {
                id = values[0] == null ? default(int?) : int.Parse(values[0]),
                matrimoni_id = int.Parse(values[1]),
                persona_id = int.Parse(values[2]),
                observacions = values[3]
            };
    }
}
