using System;
using System.Collections.Generic;
using System.Text;

using SharpMapper.ConnectionProvider;

namespace SharpMapper.SqlCommand
{
    /// <summary>
    /// Create general SQL commands.
    /// </summary>
    internal class SqlCommandHelper
    {
        #region Members and properties

        private ProviderConfiguration m_configuration;
        private TableInformation m_tableInformation;

        #endregion Members and properties

        #region Ctor

        public SqlCommandHelper(ProviderConfiguration p_configuration, TableInformation p_tableInformation)
        {
            if (p_configuration == null || (p_tableInformation.Fields == null || p_tableInformation.Fields.Length == 0))
                throw new ArgumentNullException("ProviderConfiguration o TableInformation nulli.");

            this.m_configuration = p_configuration;
            this.m_tableInformation = p_tableInformation;
        }

        #endregion Ctor

        #region Sql commands

        #region SELECT

        public string CreateSelectByPrimaryKey()
        {            
            StringBuilder t_sql = new StringBuilder();

            t_sql.Append("SELECT ");

            foreach (FieldInformation t_currFiledInformation in m_tableInformation.Fields)
            {
                t_sql.AppendFormat("{0}.{1} as {1},", m_tableInformation.Name, t_currFiledInformation.Name);
            }

            t_sql.Remove(t_sql.Length - 1, 1);

            t_sql.AppendFormat(@" FROM {0} WHERE {0}.{1} = {2}",
                m_tableInformation.Name,
                m_tableInformation.PrimaryKey.Name,
                this.CreateParameterName(m_tableInformation.PrimaryKey.Name));

            return t_sql.ToString();
        }

        public string CreateSelectByWhere(string p_sqlWhere)
        {
            if (String.IsNullOrEmpty(p_sqlWhere))
                throw new ArgumentNullException("Clausola WHERE mancante.");

            StringBuilder t_sql = new StringBuilder();

            t_sql.Append("SELECT ");

            foreach (FieldInformation t_currFiledInformation in m_tableInformation.Fields)
            {
                t_sql.AppendFormat("{0}.{1} as {1},", m_tableInformation.Name, t_currFiledInformation.Name);
            }

            t_sql.Remove(t_sql.Length - 1, 1);

            t_sql.AppendFormat(@" FROM {0} ", m_tableInformation.Name);
            t_sql.AppendFormat(" WHERE {0}", p_sqlWhere);

            return t_sql.ToString();
        }

        public string CreateSelectForList()
        {
            StringBuilder t_sql = new StringBuilder();

            t_sql.Append("SELECT ");

            foreach (FieldInformation t_currFiledInformation in m_tableInformation.Fields)
            {
                t_sql.AppendFormat("{0}.{1} as {1},", m_tableInformation.Name, t_currFiledInformation.Name);
            }

            t_sql.Remove(t_sql.Length - 1, 1);

            t_sql.AppendFormat(@" FROM {0}", m_tableInformation.Name);

            return t_sql.ToString();
        }

        #endregion SELECT

        #region DELETE

        public string CreateDeleteByPrimaryKey()
        {
            StringBuilder t_sql = new StringBuilder();

            t_sql.AppendFormat("DELETE FROM {0} WHERE {1} = {1}", m_tableInformation.Name, m_tableInformation.PrimaryKey.Name);

            return t_sql.ToString();
        }

        public string CreateDeleteByWhere(string p_sqlWhere)
        {
            if (String.IsNullOrEmpty(p_sqlWhere))
                throw new ArgumentNullException("Clausola WHERE mancante.");

            StringBuilder t_sql = new StringBuilder();

            t_sql.AppendFormat("DELETE FROM {0}", m_tableInformation.Name);
            t_sql.AppendFormat(" WHERE {0}", p_sqlWhere);

            return t_sql.ToString();
        }

        #endregion DELETE

        #region INSERT

        public string CreateInsert(object p_object)
        {
            if (p_object == null)
                throw new ArgumentNullException("p_object è nullo.");

            StringBuilder t_sql = new StringBuilder();

            t_sql.AppendFormat("INSERT INTO {0}(", m_tableInformation.Name);

            foreach (FieldInformation t_currFieldInformation in m_tableInformation.Fields)
            {
                if (t_currFieldInformation.IsPrimaryKey)
                    continue;

                t_sql.Append(t_currFieldInformation.Name).Append(',');
            }

            t_sql.Remove(t_sql.Length - 1, 1);
            t_sql.Append(") VALUES(");

            foreach (FieldInformation t_currFieldinformation in m_tableInformation.Fields)
            {
                if (t_currFieldinformation.IsPrimaryKey)
                    continue;

                t_sql.Append(this.CreateParameterName(t_currFieldinformation.Name)).Append(',');
            }

            t_sql.Remove(t_sql.Length - 1, 1);
            t_sql.Append(")");

            return t_sql.ToString();
        }

        #endregion INSERT

        #region UPDATE

        public string CreateUpdate(object p_object)
        {
            if (p_object == null)
                throw new ArgumentNullException("p_object è nullo.");

            StringBuilder t_sql = new StringBuilder();

            t_sql.AppendFormat("UPDATE {0} SET ", m_tableInformation.Name);

            foreach (FieldInformation t_currFieldinformation in m_tableInformation.Fields)
            {
                if (t_currFieldinformation.IsPrimaryKey)
                    continue;

                t_sql.AppendFormat("{0} = {1}", t_currFieldinformation.Name, this.CreateParameterName(t_currFieldinformation.Name)).Append(',');
            }

            t_sql.Remove(t_sql.Length - 1, 1);
            t_sql.AppendFormat(" WHERE {0} = {1}", m_tableInformation.PrimaryKey.Name, this.CreateParameterName(m_tableInformation.PrimaryKey.Name));

            return t_sql.ToString();
        }

        #endregion UPDATE

        #region LAST INSERT ID

        public string CreateGetLastInsertId(string p_keyName)
        {
            string t_return = String.Empty;

            switch (m_configuration.ProviderType)
            {
                case SharpMapper.Enum.ConnectionProviderEnum.MSAccessProvider:
                    t_return = String.Format("SELECT MAX({0})", p_keyName);
                    break;

                case SharpMapper.Enum.ConnectionProviderEnum.MySqlProvider:
                    t_return = "SELECT LAST_INSERT_ID()";
                    break;

                case SharpMapper.Enum.ConnectionProviderEnum.SQLServerProvider:
                    t_return = "SELECT @@IDENTITY";
                    break;
            }

            return t_return;
        }

        #endregion LAST INSERT ID

        #endregion Sql commands

        #region Helpers

        public string CreateParameterName(string p_parameterName)
        {
            string t_parameterName = String.Empty;

            t_parameterName = String.Format("{0}p_{1}", m_configuration.ParameterPrefix, p_parameterName);

            return t_parameterName;
        }

        #endregion Helpers
    }
}