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
        private PersonaRepository _personaRepository;

        public PersonDataSanitizer(string connStr)
        {
            _personaRepository = new PersonaRepository(connStr);
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

        private void TrimFields(Persona persona)
        {
            persona.nom = FirstCharToUpper(persona.nom.Trim());
            persona.llinatge_1 = FirstCharToUpper(persona.llinatge_1.Trim());
            persona.llinatge_2 = FirstCharToUpper(persona.llinatge_2.Trim());
            persona.naixement_data = FirstCharToUpper(persona.naixement_data.Trim());
            persona.naixement_lloc = FirstCharToUpper(persona.naixement_lloc.Trim());
            persona.mort_data = persona.mort_data.Trim();
            persona.mort_lloc = FirstCharToUpper(persona.mort_lloc.Trim());
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
