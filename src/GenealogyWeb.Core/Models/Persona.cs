using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Persona
    {
        [Display(Name = "ID")]
        public int? id { get; set; }
        [Display(Name = "Is male?")]
        public bool home { get; set; }
        [Display(Name = "First name")]
        public string nom { get; set; }
        [Display(Name = "Father's surname")]
        public string llinatge_1 { get; set; }
        [Display(Name = "Mothers's surname")]
        public string llinatge_2 { get; set; }
        [Display(Name = "Birth date (yyyy-MM-dd)")]
        public string naixement_data { get; set; }
        [Display(Name = "Birth place")]
        public string naixement_lloc { get; set; }
        [Display(Name = "Death date (yyyy-MM-dd)")]
        public string mort_data { get; set; }
        [Display(Name = "Death place")]
        public string mort_lloc { get; set; }
        [Display(Name = "Info")]
        public string info { get; set; }
        [Display(Name = "Observations")]
        public string observacions { get; set; }

        public string FullName { get { return $"{nom}/{llinatge_1}/{llinatge_2} (id={id})"; } }

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
