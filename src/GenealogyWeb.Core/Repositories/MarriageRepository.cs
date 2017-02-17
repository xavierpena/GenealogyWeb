using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class MarriageRepository
    {
        public const string TableName = "marriages";

        private IDbConnection _db;        
        public MarriageRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Marriage> GetAll()
            => _db.Query<Marriage>($"SELECT * FROM {TableName}");

        public Marriage GetById(int id)
            => _db.Query<Marriage>($"SELECT * FROM {TableName} WHERE {nameof(Marriage.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Marriage.id)}={id}");

        public IEnumerable<Marriage> GetAllByHusbandId(int id)
            => _db.Query<Marriage>($"SELECT * FROM {TableName} WHERE {nameof(Marriage.husband_id)}={id}");

        public IEnumerable<Marriage> GetAllByWifeId(int id)
            => _db.Query<Marriage>($"SELECT * FROM {TableName} WHERE {nameof(Marriage.wife_id)}={id}");

        public Marriage Add(Marriage marriage)
        {
            var id = _db.Query<int>(
                      $"INSERT INTO {TableName}"
                      + $" ({string.Join(",", GetNames().Select(x => $"{x}"))})"
                      + $" VALUES ({string.Join(",", GetNames().Select(x => $"@{x}"))});"
                      + $" SELECT LAST_INSERT_ID();",
                      GetObj(marriage)
                  ).Single();

            marriage.id = id;
            return marriage;
        }

        public void Update(Marriage marriage)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))}"
                      + $" WHERE {nameof(Marriage.id)}={marriage.id}",
                      GetObj(marriage)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Marriage.husband_id),
                    nameof(Marriage.wife_id),
                    nameof(Marriage.date),
                    nameof(Marriage.place),
                    nameof(Marriage.comments)
                };

        private object GetObj(Marriage marriage) => new
        {
            husband_id = marriage.husband_id,
            wife_id = marriage.wife_id,
            date = marriage.date,
            place = marriage.place,
            comments = marriage.comments
        };

        #endregion
    }
}
