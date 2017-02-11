using GenealogyWeb.Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Repositories
{
    public class MatrimoniRepository
    {
        public const string TableName = "matrimonis";

        private IDbConnection _db;        
        public MatrimoniRepository(string connStr)
        {
            _db = DbConnectionFactory.GetMysqlConnection(connStr);
        }

        public IEnumerable<Matrimoni> GetAll()
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName}");

        public Matrimoni GetById(int id)
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName} WHERE {nameof(Matrimoni.id)}={id}").FirstOrDefault();

        public int RemoveById(int id)
            => _db.Execute($"DELETE FROM {TableName} WHERE {nameof(Matrimoni.id)}={id}");

        public IEnumerable<Matrimoni> GetAllByHusbandId(int id)
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName} WHERE {nameof(Matrimoni.home_id)}={id}");

        public IEnumerable<Matrimoni> GetAllByWifeId(int id)
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName} WHERE {nameof(Matrimoni.dona_id)}={id}");

        public Matrimoni Add(Matrimoni marriage)
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

        public void Update(Matrimoni marriage)
            => _db.Execute(
                      $"UPDATE {TableName}"
                      + $" SET {string.Join(",", GetNames().Select(x => $"{x}=@{x}"))})"
                      + $" WHERE {nameof(Matrimoni.id)}={marriage.id}",
                      GetObj(marriage)
                  );

        #region "HELPERS"

        private string[] GetNames() => new string[] {
                    nameof(Matrimoni.home_id),
                    nameof(Matrimoni.dona_id),
                    nameof(Matrimoni.data),
                    nameof(Matrimoni.lloc),
                    nameof(Matrimoni.observacions)
                };

        private object GetObj(Matrimoni marriage) => new
        {
            nom = marriage.home_id,
            llinatge_1 = marriage.dona_id,
            llinatge_2 = marriage.data,
            naixement_data = marriage.lloc,
            naixement_lloc = marriage.observacions
        };

        #endregion
    }
}
