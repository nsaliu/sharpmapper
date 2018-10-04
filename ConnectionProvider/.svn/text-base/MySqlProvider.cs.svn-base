using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace SharpMapper.ConnectionProvider
{
    internal class MySqlProvider : IConnectionProvider
    {
        public IDbConnection GetConnection()
        {
            IDbConnection t_connection = new MySql.Data.MySqlClient.MySqlConnection();

            return t_connection;
        }
    }
}