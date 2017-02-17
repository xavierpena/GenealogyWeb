using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Models
{
    public class Person
    {
        [Display(Name = "ID")]
        public int? id { get; set; }
        [Display(Name = "Is male?")]
        public bool is_male { get; set; }
        [Display(Name = "First name")]
        public string name { get; set; }
        [Display(Name = "Father's surname")]
        public string father_surname { get; set; }
        [Display(Name = "Mothers's surname")]
        public string mother_surname { get; set; }
        [Display(Name = "Birth date (yyyy-MM-dd)")]
        public string birth_date { get; set; }
        [Display(Name = "Birth place")]
        public string birth_place { get; set; }
        [Display(Name = "Death date (yyyy-MM-dd)")]
        public string death_date { get; set; }
        [Display(Name = "Death place")]
        public string death_place { get; set; }
        [Display(Name = "Info")]
        public string info { get; set; }
        [Display(Name = "Comments")]
        public string comments { get; set; }

        public string FullName { get { return $"{ (is_male ? "♂" : "♀") } {name}/{father_surname}/{mother_surname} (id={id})"; } }

        // ===

        public override string ToString()
        {
            var naixement = $"{birth_place} {birth_date}";
            naixement = (naixement == " ") ? "-" : naixement;

            var mort = $"{death_place} {death_date}";
            mort = (mort == " ") ? "-" : mort;

            return $"{id}; {name}/{father_surname}/{mother_surname}; {naixement}; {mort};"; 
        }

    }
}
