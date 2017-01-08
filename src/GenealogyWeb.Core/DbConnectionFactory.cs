using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core
{
    /// <summary>
    /// Pattern example:
    /// https://code.msdn.microsoft.com/Dapper-and-Repository-68710cd7
    /// </summary>
    public static class DbConnectionFactory
    {

        /// <typeparam name="TDbConnection">Connection type (MySqlConnection, PostgresqlConnection, etc). Any accepted type by Dapper.</typeparam>
        /// <param name="connectionString">id for the connection string in app.config</param>
        public static IDbConnection GetConnection<TDbConnection>(string connectionString) where TDbConnection : IDbConnection, new()
        {
            var connection = new TDbConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }

        public static IDbConnection GetMysqlConnection(string connectionString) => GetConnection<MySqlConnection>(connectionString);

    }
}
