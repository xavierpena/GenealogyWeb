using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business.UpwardTree
{
    public class UpwardTreeBuilder
    {
        private PersonaRepository _personRepository;
        private MatrimoniRepository _marriageRepository;
        private FillRepository _sonRepository;

        private List<Persona> _persons;
        private List<Matrimoni> _marriages;
        private List<Fill> _sons;

        private Dictionary<Persona, JsonItem> _processed;

        public UpwardTreeBuilder(
            PersonaRepository personRepository,
            MatrimoniRepository marriageRepository,
            FillRepository sonRepository)
        {
            _personRepository = personRepository;
            _marriageRepository = marriageRepository;
            _sonRepository = sonRepository;
        }

        private void Init()
        {
            _processed = new Dictionary<Persona, JsonItem>();

            _persons = _personRepository.GetAll().ToList();
            _marriages = _marriageRepository.GetAll().ToList();
            _sons = _sonRepository.GetAll().ToList();
        }

        public JsonItem GetResult(int personId)
        {
            Init();
            var person = _persons.Where(x => x.id == personId).Single();
            var personNode = GetDeepNode(person);
            return personNode;
        }

        private JsonItem GetDeepNode(Persona person)
        {
            if (_processed.ContainsKey(person))
                return new JsonItem(_processed[person]);

            var personNode = new JsonItem(GetPersonDescription(person));

            _processed.Add(person, personNode);

            var sonOf = _sons.Where(x => x.persona_id == person.id).SingleOrDefault();

            if(sonOf != null)
            {
                var marriage = _marriages.Where(x => x.id == sonOf.matrimoni_id).Single();
                var father = _persons.Where(x => x.id == marriage.home_id).SingleOrDefault();
                var mother = _persons.Where(x => x.id == marriage.dona_id).SingleOrDefault();

                var marriageNode = new JsonItem("⚭");
                personNode.AddChild(marriageNode);

                var fatherNode = default(JsonItem);
                var motherNode = default(JsonItem);
                if (father != null)
                {
                    fatherNode = GetDeepNode(father);
                    marriageNode.AddChild(fatherNode);
                }
                    
                if (mother != null)
                {
                    motherNode = GetDeepNode(mother);
                    marriageNode.AddChild(motherNode);
                }                                    
            }

            return personNode;
        }        

        private string GetPersonDescription(Persona person)
            => $"{GetStr(person.nom)}/{GetStr(person.llinatge_1)}/{GetStr(person.llinatge_2)} ({GetStr(person.naixement_data) }=>{GetStr(person.mort_data)}) id={person.id}";

        private string GetStr(string inputStr)
        {
            var unknownStr = "✗";

            if (inputStr == null)
                return unknownStr;

            inputStr = inputStr.Trim();
            if (inputStr == "")
                return unknownStr;

            return inputStr;
        }

    }
}
