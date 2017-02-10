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
    }
}
