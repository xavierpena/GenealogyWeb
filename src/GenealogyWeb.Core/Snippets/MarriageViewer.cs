using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Snippets
{
    public class MarriageViewer
    {
        private Marriage _matrimoni;

        private PersonRepository _personaRepository;
        private MarriageRepository _matrimoniRepository;
        private SonRepository _fillRepository;

        private Person Home;
        private Person Dona;
        private IEnumerable<Person> Fills;

        public MarriageViewer(string connStr, Marriage matrimoni)
        {
            _personaRepository = new PersonRepository(connStr);
            _matrimoniRepository = new MarriageRepository(connStr);
            _fillRepository = new SonRepository(connStr);

            _matrimoni = matrimoni;
        }

        public static string GetAllInfos(string connStr)
        {
            var matrimoniRepository = new MarriageRepository(connStr);
            var matrimonis = matrimoniRepository.GetAll();
            var results = new List<string>();
            foreach (var matrimoni in matrimonis)
            {
                var marriageViewer = new MarriageViewer(connStr, matrimoni);
                var result = marriageViewer.GetInfo();

                results.Add("");
                results.Add("===");
                results.Add(result);
            }
            return string.Join("\r\n", results);
        }

        public string GetInfo()
        {
            Process();

            var result = new List<string>();

            result.Add($"Matrimoni: {_matrimoni.ToString()}");
            
            var homeStr = Home == null ? "NULL" : Home.ToString();
            result.Add($"  > Home: {homeStr}");

            var donaStr = Dona == null ? "NULL" : Dona.ToString();
            result.Add($"  > Dona: {donaStr}");

            var fillsStr = Fills == null ? "NULL" : $"{Fills.Count()}";
            result.Add($"  > Fills: {fillsStr}");

            if(Fills != null)
            {
                var count = 0;
                foreach(var fill in Fills)
                {
                    count += 1;
                    result.Add($"    #{count}. {fill.ToString()}");
                }
            }

            return string.Join("\r\n", result);
        }

        private void Process()
        {
            if (_matrimoni.husband_id != null)
                Home = _personaRepository.GetById(_matrimoni.husband_id.Value);

            if (_matrimoni.wife_id != null)
                Dona = _personaRepository.GetById(_matrimoni.wife_id.Value);

            var fills = _fillRepository.GetAllByMarriageId(_matrimoni.id.Value);
            if (fills != null)
            {
                Fills = _personaRepository.GetAllByIds(fills.Select(x => x.person_id));
            }
        }

    }
}
