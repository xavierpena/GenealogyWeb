using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Persona
    {
        public int? id { get; set; }
        public string generacio { get; set; }
        public string id_6 { get; set; }
        public string id_5 { get; set; }
        public string id_4 { get; set; }
        public string id_3 { get; set; }
        public string id_2 { get; set; }
        public string id_1 { get; set; }
        public bool home { get; set; }
        public string nom { get; set; }
        public string llinatge_1 { get; set; }
        public string llinatge_2 { get; set; }
        public string naixement_data { get; set; }
        public string naixement_lloc { get; set; }
        public string matrimoni_data { get; set; }
        public string matrimoni_edat { get; set; }
        public string matrimoni_lloc { get; set; }
        public string fills_nombre { get; set; }
        public string mort_data { get; set; }
        public string mort_edat { get; set; }
        public string mort_lloc { get; set; }
        public string info { get; set; }
        public string observacions { get; set; }

        // ===

        public override string ToString()
        {
            var naixement = $"{naixement_lloc} {naixement_data}";
            naixement = (naixement == " ") ? "-" : naixement;

            var mort = $"{mort_lloc} {mort_data}";
            mort = (mort == " ") ? "-" : mort;

            return $"{id}; {nom}/{llinatge_1}/{llinatge_2}; {naixement}; {mort};"; 
        }

        public string GetSearchKey()
            => $"id:{id}/{nom}/{llinatge_1}/{llinatge_2}";
        
        public static Persona PersonaFromArray(string[] values)
            => new Persona
            {
                id = values[0] == null ? default(int?) : int.Parse(values[0]),
                nom = values[1],
                llinatge_1 = values[2],
                llinatge_2 = values[3],
                home = bool.Parse(values[4]),
                naixement_data = values[5],
                naixement_lloc = values[6],
                mort_data = values[7],
                mort_lloc = values[8],
                info = values[9],
                observacions = values[10]
            };

    }
}
