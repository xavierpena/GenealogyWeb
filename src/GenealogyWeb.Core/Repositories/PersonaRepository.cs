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

        public IEnumerable<Persona> GetAllByIds(IEnumerable<int> ids)
        {
            if (!ids.Any())
                return null;
            return _db.Query<Persona>($"SELECT * FROM {TableName} WHERE {nameof(Persona.id)} IN ({string.Join(",", ids)})");
        }

        //public void Update(Persona persona)
        //    => _db.Execute($"UPDATE {TableName} SET"
        //            + $" {nameof(Persona.nom)}='{persona.nom}'"
        //            + $", {nameof(Persona.llinatge_1)}='{persona.llinatge_1}'"
        //            + $", {nameof(Persona.llinatge_2)}='{persona.llinatge_2}'"
        //            + $", {nameof(Persona.naixement_data)}='{persona.naixement_data}'"
        //            + $", {nameof(Persona.naixement_lloc)}='{persona.naixement_lloc}'"
        //            + $", {nameof(Persona.mort_data)}='{persona.mort_data}'"
        //            + $", {nameof(Persona.mort_lloc)}='{persona.mort_lloc}'"
        //            + $", {nameof(Persona.info)}='{persona.info}'"
        //            + $" WHERE {nameof(Persona.id)}={persona.id}"
        //        );

        public void Update(Persona persona)
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

    }
}
