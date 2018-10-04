using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;

namespace SharpMapper.ConnectionProvider
{
    using SharpMapper.Enum;

    internal class ConnectionFactory
    {
        public static IDbConnection InstantiateConnection(ProviderConfiguration p_providerSettings)
        {
            IConnectionProvider t_provider = null;

            switch (p_providerSettings.ProviderType)
            {
                case ConnectionProviderEnum.MSAccessProvider:
                    t_provider = new MSAccessProvider();
                    break;

                case ConnectionProviderEnum.MySqlProvider:
                    t_provider = new MySqlProvider();
                    break;

                case ConnectionProviderEnum.SQLServerProvider:
                    t_provider = new SQLServerProvider();
                    break;
            }

            IDbConnection t_connection = t_provider.GetConnection();
            return t_connection;
        }
    }
}