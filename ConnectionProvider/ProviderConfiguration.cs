using System;
using System.Collections.Generic;
using System.Text;

using SharpMapper.Enum;

namespace SharpMapper.ConnectionProvider
{
    internal class ProviderConfiguration
    {
        #region Members and properties

        private ConnectionProviderEnum m_providerType = ConnectionProviderEnum.NullProvider;
        public ConnectionProviderEnum ProviderType
        {
            get { return m_providerType; }
        }

        private string m_parameterPrefix = String.Empty;
        public string ParameterPrefix
        {
            get { return m_parameterPrefix; }
        }

        #endregion Members and properties

        #region Ctor

        public ProviderConfiguration(ConnectionProviderEnum p_providerType)
        {
            if (p_providerType == ConnectionProviderEnum.NullProvider)
            {
                throw new ArgumentException("Il tipo di Provider non può essere nullo.");
            }

            this.m_providerType = p_providerType;
            this.SetParameterPrefix();
        }

        #endregion Ctor

        #region Helpers

        private void SetParameterPrefix()
        {
            switch (m_providerType)
            {
                case ConnectionProviderEnum.MSAccessProvider:
                    this.m_parameterPrefix = "@";
                    break;

                case ConnectionProviderEnum.MySqlProvider:
                    this.m_parameterPrefix = "?";
                    break;

                case ConnectionProviderEnum.SQLServerProvider:
                    this.m_parameterPrefix = "@";
                    break;
            }
        }

        #endregion Helpers
    }
}