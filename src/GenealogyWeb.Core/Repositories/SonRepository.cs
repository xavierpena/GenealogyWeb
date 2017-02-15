using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class SonRepository
    {
        public const string TableName = "sons";

        private IDbConnection _db;        
        public SonRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Son> GetAll()
            => _db.Query<Son>($"SELECT * FROM {TableName}");

        public Son GetById(int id)
            => _db.Query<Son>($"SELECT * FROM {TableName} WHERE {nameof(Son.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Son.id)}={id}");

        public IEnumerable<Son> GetAllByMarriageId(int id)
            => _db.Query<Son>($"SELECT * FROM {TableName} WHERE {nameof(Son.marriage_id)}={id}");

        public Son GetByPersonId(int id)
            => _db.Query<Son>($"SELECT * FROM {TableName} WHERE {nameof(Son.person_id)}={id}")
            .FirstOrDefault();

        public Son Add(Son son)
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

        public void Update(Son son)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))})"
                      + $" WHERE {nameof(Son.id)}={son.id}",
                      GetObj(son)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Son.marriage_id),
                    nameof(Son.person_id),
                    nameof(Son.comments)
                };

        private object GetObj(Son son) => new
        {
            marriage_id = son.marriage_id,
            person_id = son.person_id,
            comments = son.comments
        };

        #endregion
    }
}
