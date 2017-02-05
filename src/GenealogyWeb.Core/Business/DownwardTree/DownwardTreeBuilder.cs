using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business.DownwardTree
{

    /// <summary>
    /// Source:
    /// https://github.com/ErikGartner/dTree
    /// </summary>
    public class DownwardTreeBuilder
    {
        private PersonaRepository _personRepository;
        private MatrimoniRepository _marriageRepository;
        private FillRepository _sonRepository;

        private List<Persona> _persons;
        private List<Matrimoni> _marriages;
        private List<Fill> _sons;

        private Dictionary<Persona, JsonItem> _processed;

        public DownwardTreeBuilder(
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

        // https://github.com/ErikGartner/dTree
        public JsonItem GetResult()
        {
            Init();

            var topLevelPersonIds = GetTopLevelPersonIds();
            var topLevelPersonNodes = new List<JsonItem>();
            foreach (var id in topLevelPersonIds)
            {
                var person = _persons.Where(x => x.id == id).Single();
                topLevelPersonNodes.Add(GetDeepNode(person));
            }

            var topNode = new JsonItem("results");
            topNode.AddChildren(topLevelPersonNodes.ToArray());

            return topNode;
        }

        private List<int?> GetTopLevelPersonIds()
        {
            var sonIds = _sons.Select(x => x.persona_id).ToList();

            var marriedMenIds = _marriages.Where(x => x.home_id != null).Select(x => x.home_id).ToList();
            var marriedWomenIds = _marriages.Where(x => x.dona_id != null).Select(x => x.dona_id).ToList();

            var noParentsMarriedMenIds = marriedMenIds.Where(x => !sonIds.Contains(x.Value)).ToList();
            var noParentsMarriedWomenIds = marriedWomenIds.Where(x => !sonIds.Contains(x.Value)).ToList();

            // Remove woman from top-level complete couples (complete = contains both man and woman):
            foreach (var marriage in _marriages)
            {
                if (noParentsMarriedWomenIds.Contains(marriage.dona_id))
                {
                    if (marriage.home_id != null)
                    {
                        noParentsMarriedWomenIds.Remove(marriage.dona_id);
                    }
                }
            }

            var topLevelPersonIds = new List<int?>();
            foreach (var id in noParentsMarriedMenIds)
                topLevelPersonIds.Add(id);
            foreach (var id in noParentsMarriedWomenIds)
                topLevelPersonIds.Add(id);

            return topLevelPersonIds
                .Distinct()
                .ToList();
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

            var personNode = new JsonItem(Utils.GetPersonDescription(person));

            _processed.Add(person, personNode);

            var marriages = _marriages
                .Where(x => x.home_id == person.id || x.dona_id == person.id)
                .ToList();

            foreach(var marriage in marriages)
            {
                var marriageNode = GetMarriageNode(person, marriage);
                
                var childrenNodes = GetChildrenNodes(marriage);
                if (childrenNodes != null)
                    marriageNode.AddChildren(childrenNodes.ToArray());

                personNode.AddChildren(marriageNode);                
            }

            return personNode;
        }

        private JsonItem GetMarriageNode(Persona person, Matrimoni marriage)
        {
            var marriageStr = Utils.GetMarriageDescription(marriage);

            var partnerId = (marriage.home_id == person.id) ? marriage.dona_id : marriage.home_id;
            var partner = _persons.Where(x => x.id == partnerId).SingleOrDefault();
            var partnerNode = default(JsonItem);
            if (partner != null)
            {
                if (_processed.ContainsKey(partner))
                    partnerNode = new JsonItem(GetFullMarriageDescription(marriageStr, Utils.GetDuplicateStr(partner)));
                else
                    partnerNode = new JsonItem(GetFullMarriageDescription(marriageStr, Utils.GetPersonDescription(partner)));
            }
            else
            {
                partnerNode = new JsonItem(GetFullMarriageDescription(marriageStr, "(unknown)"));
            }

            return partnerNode;
        }

        private string GetFullMarriageDescription(string marriageStr, string partnerStr)
            => $"{marriageStr} w/ {partnerStr}";

        private List<JsonItem> GetChildrenNodes(Matrimoni marriage)
        {
            var childrenNodes = default(List<JsonItem>);
            var sons = _sons.Where(x => x.matrimoni_id == marriage.id).ToList();
            if (sons.Any())
            {
                childrenNodes = new List<JsonItem>();
                foreach (var son in sons)
                {
                    var sonPerson = _persons.Where(x => x.id == son.persona_id).Single();
                    var sonNode = default(JsonItem);
                    if (_processed.ContainsKey(sonPerson))
                        sonNode = new JsonItem(_processed[sonPerson]);
                    else
                        sonNode = GetDeepNode(sonPerson);
                    childrenNodes.Add(sonNode);
                }
            }

            return childrenNodes;
        }
    }
}
