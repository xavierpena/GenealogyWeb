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
        public static List<SelectListItem> GetPersonsSelectList(IEnumerable<Person> persons)
            => persons
                .Select(x => new SelectListItem {
                    Text = x.FullName,
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetPersonsSelectListByGender(IEnumerable<Person> persons, bool isMale)
            => persons
                .Where(x => x.is_male == isMale)
                .Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetMarriagesSelectList(IEnumerable<Marriage> marriages, IEnumerable<Person> persons)
            => marriages
                .Select(x => new SelectListItem
                {
                    Text = GetMarriageStr(persons, x.husband_id, x.wife_id),
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        public static List<SelectListItem> GetSonsSelectList(IEnumerable<Son> sons, IEnumerable<Marriage> marriages, IEnumerable<Person> persons)
            => sons
                .Select(x => new SelectListItem
                {
                    Text = GetSonStr(x, persons, marriages),
                    Value = x.id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

        #region "Internal formatters" 

        private static string GetMarriageStr(IEnumerable<Person> persons, int? home_id, int? dona_id)
        {
            var husband = persons.Where(x => x.id == home_id).SingleOrDefault();
            var wife = persons.Where(x => x.id == dona_id).SingleOrDefault();

            return $"{husband?.FullName} & {wife?.FullName}";
        }

        private static string GetSonStr(Son son, IEnumerable<Person> persons, IEnumerable<Marriage> marriages)   
        {
            var person = persons.Where(x => x.id == son.person_id).Single();

            var marriage = marriages.Where(x => x.id == son.marriage_id).Single();

            var husband = persons.Where(x => x.id == marriage.husband_id).SingleOrDefault();
            var wife = persons.Where(x => x.id == marriage.wife_id).SingleOrDefault();

            return $"{husband?.FullName} & {wife?.FullName} => {person.FullName}";
        }

        #endregion
    }
}
