using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class PersonaRepository
    {
        public const string TableName = "persones";

        private IDbConnection _db;        
        public PersonaRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Persona> GetAll()
            => _db.Query<Persona>($"SELECT * FROM {TableName}");

        public IEnumerable<Persona> GetPersonesNotInFills()
            => _db.Query<Persona>(
                    $"SELECT * FROM {TableName}"
                    + $" WHERE {nameof(Persona.id)} NOT IN" 
                    + $" (SELECT {nameof(Fill.persona_id)} FROM {FillRepository.TableName});"
                );

        public Persona GetById(int id)
            => _db.Query<Persona>($"SELECT * FROM {TableName} WHERE {nameof(Persona.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Persona.id)}={id}");

        public IEnumerable<Persona> GetAllByIds(IEnumerable<int> ids)
        {
            if (!ids.Any())
                return null;
            return _db.Query<Persona>($"SELECT * FROM {TableName} WHERE {nameof(Persona.id)} IN ({string.Join(",", ids)})");
        }

        public void Update1(Persona persona)
        {
            var test = $"UPDATE {TableName} SET"
            + $" {nameof(Persona.nom)}=\"{persona.nom}\""
            + $", {nameof(Persona.llinatge_1)}=\"{persona.llinatge_1}\""
            + $", {nameof(Persona.llinatge_2)}=\"{persona.llinatge_2}\""
            + $", {nameof(Persona.naixement_data)}=\"{persona.naixement_data}\""
            + $", {nameof(Persona.naixement_lloc)}=\"{persona.naixement_lloc}\""
            + $", {nameof(Persona.mort_data)}=\"{persona.mort_data}\""
            + $", {nameof(Persona.mort_lloc)}=\"{persona.mort_lloc}\""
            + $", {nameof(Persona.info)}=\"{persona.info}\""
            + $" WHERE {nameof(Persona.id)}={persona.id}";

            _db.Execute(test);
        }

        public Persona Add(Persona person)
        {
            var id = _db.Query<int>(
                      $"INSERT INTO {TableName}"
                      + $" ({string.Join(",", GetNames().Select(x => $"{x}"))})"
                      + $" VALUES ({string.Join(",", GetNames().Select(x => $"@{x}"))});"
                      + $" SELECT LAST_INSERT_ID();",
                      GetObj(person)
                  ).Single();

            person.id = id;
            return person;
        }

        public void Update(Persona person)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))})"
                      + $" WHERE {nameof(Persona.id)}={person.id}",
                      GetObj(person)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Persona.nom),
                    nameof(Persona.llinatge_1),
                    nameof(Persona.llinatge_2),
                    nameof(Persona.naixement_data),
                    nameof(Persona.naixement_lloc),
                    nameof(Persona.mort_data),
                    nameof(Persona.mort_lloc),
                    nameof(Persona.info)
                };

        private object GetObj(Persona person) => new
        {
            nom = person.nom,
            llinatge_1 = person.llinatge_1,
            llinatge_2 = person.llinatge_2,
            naixement_data = person.naixement_data,
            naixement_lloc = person.naixement_lloc,
            mort_data = person.mort_data,
            mort_lloc = person.mort_lloc,
            info = person.info
        };

        #endregion
    }
}
