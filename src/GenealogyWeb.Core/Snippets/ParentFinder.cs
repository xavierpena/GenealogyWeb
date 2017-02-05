//using GenealogyWeb.Core.Models;
//using GenealogyWeb.Core.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace GenealogyWeb.Core.Snippets
//{
//    /// <summary>
//    /// Gets all the persons without a record in "fills". Tries to find the double of their internal id.
//    /// The internal id can be either id_1 or id_2 (it has to be changed manually).
//    /// If the marriage is not found, the marriage is inserted. This means that the script must be ran again after the insertion (to link son and marriage).
//    /// </summary>
//    public class ParentFinder
//    {
//        private PersonaRepository _personaRepository ;
//        private MatrimoniRepository _matrimoniRepository;
//        private FillRepository _fillRepository;

//        public ParentFinder(string connStr)
//        {
//            _personaRepository = new PersonaRepository(connStr);
//            _matrimoniRepository = new MatrimoniRepository(connStr);
//            _fillRepository = new FillRepository(connStr);
//        }

//        public string GetResults()
//        {
//            var persones = _personaRepository.GetAll();
//            var matrimonis = _matrimoniRepository.GetAll();
//            var fills = _fillRepository.GetAll();

//            var personesNotInFills = _personaRepository.GetPersonesNotInFills();

//            var results = new List<string>();
//            foreach (var persona in personesNotInFills)
//            {
//                var parent = FindParent(persona, persones);
//                if (parent != null)
//                {
//                    var matrimoni = matrimonis.Where(x => x.home_id == parent.id).FirstOrDefault();
//                    if (matrimoni != null)
//                        results.Add($"INSERT INTO fills (matrimoni_id,persona_id) VALUES({matrimoni.id},{persona.id});");
//                    else
//                        results.Add($"INSERT INTO matrimonis (home_id, dona_id, data, lloc, observacions) VALUES({parent.id},NULL,{GetValueOrNullStr(parent.matrimoni_data)},{GetValueOrNullStr(parent.matrimoni_lloc)},NULL);");
//                }
//            }

//            var resultStr = string.Join("\r\n", results);

//            return resultStr;
//        }

//        private static string GetValueOrNullStr(string input)
//        {
//            if (input == null)
//                return "NULL";
//            if (input == "")
//                return "NULL";
//            return input;
//        }

//        private static Persona FindParent(Persona persona, IEnumerable<Persona> persones)
//        {
//            var personInternalId = GetCleanId(persona.id_2);

//            // Only even values (=male) have a parent in this list:
//            if (personInternalId % 2 != 0)
//                return null;

//            if (personInternalId > -1)
//            {
//                var parentId = 2 * personInternalId;
//                var parent = persones.Where(x => GetCleanId(x.id_2) == parentId).FirstOrDefault();
//                return parent;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        private static int GetCleanId(string rawId)
//            => rawId == "" ? -1 : int.Parse(rawId.Trim().Split(' ')[0]);
//    }
//}
