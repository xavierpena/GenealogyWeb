using GenealogyWeb.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Controllers
{
    public class Utils
    {
        public static List<SelectListItem> GetPersonsSelectList(IEnumerable<Persona> persons)
            => persons
                .Select(x => new SelectListItem {
                    Text = x.FullName,
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetPersonsSelectListByGender(IEnumerable<Persona> persons, bool isMale)
            => persons
                .Where(x => x.home == isMale)
                .Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetMarriagesSelectList(IEnumerable<Matrimoni> marriages, IEnumerable<Persona> persons)
            => marriages
                .Select(x => new SelectListItem
                {
                    Text = GetMarriageStr(persons, x.home_id, x.dona_id),
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetSonsSelectList(IEnumerable<Fill> sons, IEnumerable<Matrimoni> marriages, IEnumerable<Persona> persons)
            => sons
                .Select(x => new SelectListItem
                {
                    Text = GetSonStr(x, persons, marriages),
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        #region "Internal formatters" 

        private static string GetMarriageStr(IEnumerable<Persona> persons, int? home_id, int? dona_id)
        {
            var husband = persons.Where(x => x.id == home_id).SingleOrDefault();
            var wife = persons.Where(x => x.id == dona_id).SingleOrDefault();

            return $"{husband?.FullName} & {wife?.FullName}";
        }

        private static string GetSonStr(Fill son, IEnumerable<Persona> persons, IEnumerable<Matrimoni> marriages)   
        {
            var person = persons.Where(x => x.id == son.persona_id).Single();

            var marriage = marriages.Where(x => x.id == son.matrimoni_id).Single();

            var husband = persons.Where(x => x.id == marriage.home_id).SingleOrDefault();
            var wife = persons.Where(x => x.id == marriage.dona_id).SingleOrDefault();

            return $"{husband?.FullName} & {wife?.FullName} => {person.FullName}";
        }

        #endregion
    }
}
