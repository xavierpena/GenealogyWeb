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
            var marriedPeopleIds = _marriages.Where(x => x.home_id != null).Select(x => x.home_id).ToList();
            marriedPeopleIds.AddRange(_marriages.Where(x => x.dona_id != null).Select(x => x.dona_id).ToList());

            var topLevelPersonIds = marriedPeopleIds.Where(x => !sonIds.Contains(x.Value)).ToList();

            var topLevelPersonNodes = new List<PersonNode>();
            foreach(var id in topLevelPersonIds)
            {
                var person = _persons.Where(x => x.id == id).Single();
                topLevelPersonNodes.Add(GetDeepNode(person));
            }

            return topLevelPersonNodes;
        }

        private PersonNode GetDeepNode(Persona person)
        {
            var personNode = new PersonNode
            {
                name = person.ToString(),
                @class = person.home ? "man" : "woman",
            };

            _processed.Add(person, personNode);

            var marriage = _marriages
                .Where(x => x.home_id == person.id || x.dona_id == person.id)
                .SingleOrDefault();

            if(marriage != null)
            {
                var partnerId = (marriage.home_id == person.id) ? marriage.dona_id : marriage.home_id;
                var partner = _persons.Where(x => x.id == partnerId).SingleOrDefault();
                var partnerNode = default(PersonNode);
                if (_processed.ContainsKey(partner))
                {
                    partnerNode = _processed[partner];
                }
                else
                {
                    partnerNode = new PersonNode
                    {
                        name = partner.ToString(),
                        @class = partner.home ? "man" : "woman",
                    };
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
                            sonNode = _processed[sonPerson];
                        else
                            sonNode = GetDeepNode(sonPerson);
                        childrenNodes.Add(sonNode);
                    }
                }

                var marriageNode = new MarriageNode
                {
                    spouse = partnerNode,
                    children = childrenNodes
                };

                personNode.marriages.Add(marriageNode);
            }
                       
            return personNode;
        }
    }
}
