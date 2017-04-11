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
        public static IDbConnection GetMysqlConnection(string connectionString) {
            var connection = new MySqlConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }       
    }
}
