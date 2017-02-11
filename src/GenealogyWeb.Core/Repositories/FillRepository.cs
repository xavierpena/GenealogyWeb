using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class FillRepository
    {
        public const string TableName = "fills";

        private IDbConnection _db;        
        public FillRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Fill> GetAll()
            => _db.Query<Fill>($"SELECT * FROM {TableName}");

        public Fill GetById(int id)
            => _db.Query<Fill>($"SELECT * FROM {TableName} WHERE {nameof(Fill.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Fill.id)}={id}");

        public IEnumerable<Fill> GetAllByMarriageId(int id)
            => _db.Query<Fill>($"SELECT * FROM {TableName} WHERE {nameof(Fill.matrimoni_id)}={id}");

        public Fill GetByPersonId(int id)
            => _db.Query<Fill>($"SELECT * FROM {TableName} WHERE {nameof(Fill.persona_id)}={id}")
            .FirstOrDefault();

        public Fill Add(Fill son)
        {
            var id = _db.Query<int>(
                      $"INSERT INTO {TableName}"
                      + $" ({string.Join(",", GetNames().Select(x => $"{x}"))})"
                      + $" VALUES ({string.Join(",", GetNames().Select(x => $"@{x}"))});"
                      + $" SELECT LAST_INSERT_ID();",
                      GetObj(son)
                  ).Single();

            son.id = id;
            return son;
        }

        public void Update(Fill son)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))})"
                      + $" WHERE {nameof(Fill.id)}={son.id}",
                      GetObj(son)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Fill.matrimoni_id),
                    nameof(Fill.persona_id),
                    nameof(Fill.observacions)
                };

        private object GetObj(Fill son) => new
        {
            nom = son.matrimoni_id,
            llinatge_1 = son.persona_id,
            llinatge_2 = son.observacions
        };

        #endregion
    }
}
