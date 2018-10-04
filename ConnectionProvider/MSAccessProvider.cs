using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace SharpMapper.ConnectionProvider
{
    internal class MSAccessProvider : IConnectionProvider
    {
        public IDbConnection GetConnection()
        {
            IDbConnection t_connection = new OleDbConnection();

            return t_connection;
        }
    }
}