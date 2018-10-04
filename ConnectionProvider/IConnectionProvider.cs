using System;
using System.Collections.Generic;
using System.Text;

using System.Data;

namespace SharpMapper.ConnectionProvider
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}