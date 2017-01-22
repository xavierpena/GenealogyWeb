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

        private const string UnknownStr = "✗";
        private const string MaleSignStr = "♂";
        private const string FemaleSignStr = "♀";
        private const string MarriageSignStr = "⚤";

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

            var personNode = new JsonItem(GetPersonDescription(person));

            _processed.Add(person, personNode);

            var sonOf = _sons.Where(x => x.persona_id == person.id).SingleOrDefault();

            if(sonOf != null)
            {
                var marriage = _marriages.Where(x => x.id == sonOf.matrimoni_id).Single();

                var father = _persons.Where(x => x.id == marriage.home_id).SingleOrDefault();
                var mother = _persons.Where(x => x.id == marriage.dona_id).SingleOrDefault();

                var marriageNode = new JsonItem($"{MarriageSignStr} Marriage @ { GetYear(GetStr(marriage.data)) }");
                personNode.AddChild(marriageNode);

                var fatherNode = default(JsonItem);                
                if (father != null)
                    fatherNode = GetDeepNode(father);
                else
                    fatherNode = new JsonItem($"{MaleSignStr} (unknown)");
                marriageNode.AddChild(fatherNode);

                var motherNode = default(JsonItem);
                if (mother != null)
                    motherNode = GetDeepNode(mother);
                else
                    motherNode = new JsonItem($"{FemaleSignStr} (unknown)");

                marriageNode.AddChild(motherNode);
                                                 
            }

            return personNode;
        }        

        /// <summary>
        /// Full person description formatted as:
        /// `male/female_sign name/surname1/surname2 (birth=>death) id=XXX`
        /// </summary>
        private string GetPersonDescription(Persona person)
            => $"{ (person.home ? MaleSignStr : FemaleSignStr) } {GetStr(person.nom)}/{GetStr(person.llinatge_1)}/{GetStr(person.llinatge_2)}" 
                + $" ({GetYear(GetStr(person.naixement_data)) }=>{GetYear(GetStr(person.mort_data))})" 
                + $" id={person.id}";

        /// <summary>
        /// Cleans the string.
        /// If the string shows no info, returns `UnknownStr`
        /// </summary>
        private string GetStr(string inputStr)
        {           
            if (inputStr == null)
                return UnknownStr;

            inputStr = inputStr.Trim();
            if (inputStr == "")
                return UnknownStr;

            return inputStr;
        }

        /// <summary>
        /// Gets only the year from the date.
        /// Expectects a date formatted as 'yyyy-MM-dd'
        /// </summary>
        private string GetYear(string inputStr)
        {
            if (inputStr != UnknownStr)
                return inputStr.Split('-')[0];
            else
                return inputStr;
        }

    }
}
