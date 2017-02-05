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

        public JsonItem GetResult(int rootPersonId)
        {
            Init();
            var person = _persons.Where(x => x.id == rootPersonId).Single();
            var personNode = GetDeepNode(person);
            return personNode;
        }

        /// <summary>
        /// Builds a JsnoItem with:
        /// 
        /// - The name of the person as the root node
        /// - His/her parent's marriage as a child node
        /// - From the marriage node:
        ///     * Add the deep node from the father
        ///     * Add the deep node from the mother
        ///     
        /// </summary>
        private JsonItem GetDeepNode(Persona person)
        {
            if (_processed.ContainsKey(person))
                return new JsonItem(_processed[person]);

            var personNode = new JsonItem(Utils.GetPersonDescription(person));

            _processed.Add(person, personNode);

            var sonOf = _sons.Where(x => x.persona_id == person.id).SingleOrDefault();

            if(sonOf != null)
            {
                var marriage = _marriages.Where(x => x.id == sonOf.matrimoni_id).Single();

                var father = _persons.Where(x => x.id == marriage.home_id).SingleOrDefault();
                var mother = _persons.Where(x => x.id == marriage.dona_id).SingleOrDefault();

                var marriageNode = new JsonItem(Utils.GetMarriageDescription(marriage));
                personNode.AddChild(marriageNode);

                var fatherNode = default(JsonItem);                
                if (father != null)
                    fatherNode = GetDeepNode(father);
                else
                    fatherNode = new JsonItem($"{Utils.MaleSignStr} (unknown)");
                marriageNode.AddChild(fatherNode);

                var motherNode = default(JsonItem);
                if (mother != null)
                    motherNode = GetDeepNode(mother);
                else
                    motherNode = new JsonItem($"{Utils.FemaleSignStr} (unknown)");

                marriageNode.AddChild(motherNode);
                                                 
            }

            return personNode;
        }        

    }
}
