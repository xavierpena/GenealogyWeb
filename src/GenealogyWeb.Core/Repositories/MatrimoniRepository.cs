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

        public IEnumerable<Matrimoni> GetAllByHusbandId(int id)
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName} WHERE {nameof(Matrimoni.home_id)}={id}");

        public IEnumerable<Matrimoni> GetAllByWifeId(int id)
            => _db.Query<Matrimoni>($"SELECT * FROM {TableName} WHERE {nameof(Matrimoni.dona_id)}={id}");
    }
}
