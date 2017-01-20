using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business.Tree
{

    /// <summary>
    /// Source:
    /// https://github.com/ErikGartner/dTree
    /// </summary>
    public class TreeBuilder
    {
        private PersonaRepository _personRepository;
        private MatrimoniRepository _marriageRepository;
        private FillRepository _sonRepository;

        private List<Persona> _persons;
        private List<Matrimoni> _marriages;
        private List<Fill> _sons;

        private Dictionary<Persona, PersonNode> _processed;

        public TreeBuilder(
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
            _processed = new Dictionary<Persona, PersonNode>();

            _persons = _personRepository.GetAll().ToList();
            _marriages = _marriageRepository.GetAll().ToList();
            _sons = _sonRepository.GetAll().ToList();
        }

        // https://github.com/ErikGartner/dTree
        public List<PersonNode> GetResult()
        {
            Init();

            var sonIds = _sons.Select(x => x.persona_id).ToList();

            var marriedMenIds = _marriages.Where(x => x.home_id != null).Select(x => x.home_id).ToList();
            var marriedWomenIds = _marriages.Where(x => x.dona_id != null).Select(x => x.dona_id).ToList();

            var noParentsMarriedMenIds = marriedMenIds.Where(x => !sonIds.Contains(x.Value)).ToList();
            var noParentsMarriedWomenIds = marriedWomenIds.Where(x => !sonIds.Contains(x.Value)).ToList();
            
            // Remove woman from top-level complete couples (complete = contains both man and woman):
            foreach(var marriage in _marriages)
            {
                if (noParentsMarriedWomenIds.Contains(marriage.dona_id))
                {
                    if(marriage.home_id != null)
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

            var topLevelPersonNodes = new List<PersonNode>();
            foreach(var id in topLevelPersonIds)
            {
                var person = _persons.Where(x => x.id == id).Single();
                topLevelPersonNodes.Add(GetDeepNode(person));
            }

            return topLevelPersonNodes;
        }

        public object GetResult(int personId)
        {
            Init();
            var person = _persons.Where(x => x.id == personId).Single();
            var personNode = GetDeepNode(person);
            return new List<PersonNode> { personNode };
        }

        private PersonNode GetDeepNode(Persona person)
        {
            if (_processed.ContainsKey(person))
                return new PersonNode(_processed[person]);

            var personNode = new PersonNode
            {
                name = person.ToString(),
                @class = person.home ? "man" : "woman",
            };

            _processed.Add(person, personNode);

            var marriages = _marriages
                .Where(x => x.home_id == person.id || x.dona_id == person.id)
                .ToList();

            foreach(var marriage in marriages)
            {
                var partnerId = (marriage.home_id == person.id) ? marriage.dona_id : marriage.home_id;
                var partner = _persons.Where(x => x.id == partnerId).SingleOrDefault();
                var partnerNode = default(PersonNode);
                if (partner != null)
                {                    
                    if (_processed.ContainsKey(partner))
                    {
                        partnerNode = new PersonNode(_processed[partner]); ;
                    }
                    else
                    {
                        partnerNode = new PersonNode
                        {
                            name = partner.ToString(),
                            @class = partner.home ? "man" : "woman",
                        };
                    }
                }

                var childrenNodes = default(List<PersonNode>);
                var sons = _sons.Where(x => x.matrimoni_id == marriage.id).ToList();
                if(sons.Any())
                {
                    childrenNodes = new List<PersonNode>();
                    foreach (var son in sons)
                    {
                        var sonPerson = _persons.Where(x => x.id == son.persona_id).Single();
                        var sonNode = default(PersonNode);
                        if (_processed.ContainsKey(sonPerson))
                            sonNode = new PersonNode(_processed[sonPerson]);
                        else
                            sonNode = GetDeepNode(sonPerson);
                        childrenNodes.Add(sonNode);
                    }
                }

                if(partnerNode != null && childrenNodes != null)
                {
                    var marriageNode = new MarriageNode
                    {
                        spouse = partnerNode,
                        children = childrenNodes
                    };

                    personNode.AddMarriage(marriageNode);
                }
            }
                       
            return personNode;
        }
    }
}
