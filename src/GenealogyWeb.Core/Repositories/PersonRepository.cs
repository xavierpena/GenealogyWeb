using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class PersonRepository
    {
        public const string TableName = "people";

        private IDbConnection _db;        
        public PersonRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Person> GetAll()
            => _db.Query<Person>($"SELECT * FROM {TableName}");

        public IEnumerable<Person> GetPersonesNotInFills()
            => _db.Query<Person>(
                    $"SELECT * FROM {TableName}"
                    + $" WHERE {nameof(Person.id)} NOT IN" 
                    + $" (SELECT {nameof(Son.person_id)} FROM {SonRepository.TableName});"
                );

        public Person GetById(int id)
            => _db.Query<Person>($"SELECT * FROM {TableName} WHERE {nameof(Person.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Person.id)}={id}");

        public IEnumerable<Person> GetAllByIds(IEnumerable<int> ids)
        {
            if (!ids.Any())
                return null;
            return _db.Query<Person>($"SELECT * FROM {TableName} WHERE {nameof(Person.id)} IN ({string.Join(",", ids)})");
        }

        public void Update1(Person persona)
        {
            var test = $"UPDATE {TableName} SET"
            + $" {nameof(Person.name)}=\"{persona.name}\""
            + $", {nameof(Person.father_surname)}=\"{persona.father_surname}\""
            + $", {nameof(Person.mother_surname)}=\"{persona.mother_surname}\""
            + $", {nameof(Person.birth_date)}=\"{persona.birth_date}\""
            + $", {nameof(Person.birth_place)}=\"{persona.birth_place}\""
            + $", {nameof(Person.death_date)}=\"{persona.death_date}\""
            + $", {nameof(Person.death_place)}=\"{persona.death_place}\""
            + $", {nameof(Person.info)}=\"{persona.info}\""
            + $" WHERE {nameof(Person.id)}={persona.id}";

            _db.Execute(test);
        }

        public Person Add(Person person)
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

        public void Update(Person person)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))})"
                      + $" WHERE {nameof(Person.id)}={person.id}",
                      GetObj(person)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Person.name),
                    nameof(Person.father_surname),
                    nameof(Person.mother_surname),
                    nameof(Person.birth_date),
                    nameof(Person.birth_place),
                    nameof(Person.death_date),
                    nameof(Person.death_place),
                    nameof(Person.info)
                };

        private object GetObj(Person person) => new
        {
            nom = person.name,
            llinatge_1 = person.father_surname,
            llinatge_2 = person.mother_surname,
            naixement_data = person.birth_date,
            naixement_lloc = person.birth_place,
            mort_data = person.death_date,
            mort_lloc = person.death_place,
            info = person.info
        };

        #endregion
    }
}
