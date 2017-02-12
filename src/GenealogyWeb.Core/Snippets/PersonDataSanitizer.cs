using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Snippets
{
    public class PersonDataSanitizer
    {
        private PersonRepository _personaRepository;

        public PersonDataSanitizer(string connStr)
        {
            _personaRepository = new PersonRepository(connStr);
        }

        public void Sanitize()
        {
            var persons = _personaRepository.GetAll().ToList();
            for(var personIndex = 0; personIndex < persons.Count(); personIndex ++)
            {
                var person = persons[personIndex];
                TrimFields(person);
                _personaRepository.Update(person);
            }
        }

        private void TrimFields(Person persona)
        {
            persona.name = FirstCharToUpper(persona.name.Trim());
            persona.father_surname = FirstCharToUpper(persona.father_surname.Trim());
            persona.mother_surname = FirstCharToUpper(persona.mother_surname.Trim());
            persona.birth_date = FirstCharToUpper(persona.birth_date.Trim());
            persona.birth_place = FirstCharToUpper(persona.birth_place.Trim());
            persona.death_date = persona.death_date.Trim();
            persona.death_place = FirstCharToUpper(persona.death_place.Trim());
            persona.info = persona.info.Trim();
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;
            if (input.Contains("."))
                return input; // Avoid accronims such as "S.E."
            var parts = input.Split(' ');
            var result = new List<string>();
            foreach(var part in parts)
            {
                if (part.Length == 0)
                    continue;
                var sanitizedPart = part.First().ToString().ToUpper() + String.Join("", part.ToLower().Skip(1));
                result.Add(sanitizedPart);
            }
            return string.Join(" ", result);
        }
    }
}
