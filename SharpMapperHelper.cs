using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;

using SharpMapper.Enum;
using SharpMapper.ConnectionProvider;
using SharpMapper.SqlCommand;

namespace SharpMapper
{
    /// <summary>
    /// Manages persistence of a class.
    /// </summary>
    public sealed class SharpMapperHelper
    {
        #region Singleton

        private static volatile SharpMapperHelper instance;
        private static object syncRoot = new Object();

        private SharpMapperHelper() { }

        public static SharpMapperHelper Instance
        {
          get 
          {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null)
                       instance = new SharpMapperHelper();
                }
             }

             return instance;
          }
        }

        #endregion Singleton

        #region Members and properties

        private Dictionary<System.Type, TableInformation> entityMappings =
            new Dictionary<Type, TableInformation>();

        private string m_connectionString = ConfigurationManager.ConnectionStrings["ProviderConnectionString"].ConnectionString;

        private ProviderConfiguration m_providerConfiguration;

        #endregion Members and properties

        #region Helpers

        private void LoadConfiguration()
        {
            ConnectionProviderEnum t_providerType = (ConnectionProviderEnum)Int32.Parse(ConfigurationSettings.AppSettings["ProviderType"]);
            m_providerConfiguration = new SharpMapper.ConnectionProvider.ProviderConfiguration(t_providerType);
        }

        public IDbConnection GetConnection()
        {
            this.LoadConfiguration();

            IDbConnection t_conn = ConnectionFactory.InstantiateConnection(m_providerConfiguration);
            t_conn.ConnectionString = m_connectionString;
            t_conn.Open();

            return t_conn;
        }

        private TableInformation MapEntity(System.Type p_type)
        {
            TableInformation t_tableInformation = new TableInformation();

            System.Attribute[] t_attributes = System.Attribute.GetCustomAttributes(p_type);

            foreach (System.Attribute t_currAttribute in t_attributes)
            {
                if (t_currAttribute is TableAttribute)
                {
                    TableAttribute t_tableAttribute = (TableAttribute)t_currAttribute;
                    t_tableInformation.Name = t_tableAttribute.TableName;
                    break;
                }
            }

            List<FieldInformation> t_fields = new List<FieldInformation>();
            foreach (System.Reflection.PropertyInfo t_currPropertyinfo in p_type.GetProperties())
            {
                System.Attribute[] t_attributeArray = System.Attribute.GetCustomAttributes(t_currPropertyinfo);

                foreach (System.Attribute t_currAttribute in t_attributeArray)
                {
                    if (t_currAttribute is FieldAttribute)
                    {
                        FieldAttribute t_fieldAttribute = (FieldAttribute)t_currAttribute;

                        FieldInformation t_fieldInformation = new FieldInformation();
                        t_fieldInformation.Name = t_fieldAttribute.FieldName;
                        t_fieldInformation.IsPrimaryKey = t_fieldAttribute.IsPrimaryKey;
                        t_fieldInformation.PropertyName = t_currPropertyinfo.Name;

                        if (t_fieldAttribute.IsPrimaryKey)
                            t_tableInformation.PrimaryKey = t_fieldInformation;

                        t_fields.Add(t_fieldInformation);
                    }
                }
            }

            t_tableInformation.Fields = t_fields.ToArray();

            return t_tableInformation;
        }

        private TableInformation GetTableInformations(Type p_targetType)
        {
            if (entityMappings.ContainsKey(p_targetType))
                return entityMappings[p_targetType];
            else
            {
                TableInformation t_tableInformation = MapEntity(p_targetType);
                entityMappings.Add(p_targetType, t_tableInformation);

                return t_tableInformation;
            }
        }

        private void PopulateEntity(IDataReader p_reader, TableInformation p_tableInformation, object p_objectToFill)
        {
            foreach (FieldInformation t_currFiledInformation in p_tableInformation.Fields)
            {
                if (p_reader[t_currFiledInformation.Name] != DBNull.Value)
                {
                    p_objectToFill.GetType().
                        GetProperty(t_currFiledInformation.PropertyName).
                        SetValue(p_objectToFill, p_reader[t_currFiledInformation.Name], null);
                }
            }
        }

        #endregion Helpers

        #region Public methods

        #region Find

        public void ExecuteFindByID(IDbTransaction p_transaction, object p_object) 
        {
            TableInformation t_tableInformation = this.GetTableInformations(p_object.GetType());
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateSelectByPrimaryKey();

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDbDataParameter t_param = cmd.CreateParameter();
            t_param.ParameterName = t_sqlHelper.CreateParameterName(t_tableInformation.PrimaryKey.Name);
            t_param.Value = p_object.GetType().GetProperty(t_tableInformation.PrimaryKey.PropertyName).GetValue(p_object, null);
            cmd.Parameters.Add(t_param);

            IDataReader t_reader = cmd.ExecuteReader();
            if (t_reader.Read())
                this.PopulateEntity(t_reader, t_tableInformation, p_object);

            t_reader.Close();
        }

        public T FindByID<T>(IDbTransaction p_transaction, object p_objectPrimaryKey)
            where T:new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateSelectByPrimaryKey();

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDbDataParameter t_param = cmd.CreateParameter();
            t_param.ParameterName = t_sqlHelper.CreateParameterName(t_tableInformation.PrimaryKey.Name);
            t_param.Value = p_objectPrimaryKey;
            cmd.Parameters.Add(t_param);

            IDataReader t_reader = cmd.ExecuteReader();
            
            T t_genericObject = default(T);
            t_genericObject = new T();

            if (t_reader.Read())
                this.PopulateEntity(t_reader, t_tableInformation, t_genericObject);

            t_reader.Close();

            return t_genericObject;
        }

        public T FindBySQL<T>(IDbTransaction p_transaction, string p_sqlWhere)
            where T : new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateSelectByWhere(p_sqlWhere);

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDataReader t_reader = cmd.ExecuteReader();
            T t_genericObject = default(T);
            t_genericObject = new T();

            if (t_reader.Read())
                this.PopulateEntity(t_reader, t_tableInformation, t_genericObject);

            t_reader.Close();

            return t_genericObject;
        }

        // Ha senso?
        public T FindBySQL<T>(IDbTransaction p_transaction, string p_sqlFrom, string p_sqlWhere)
            where T : new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            StringBuilder t_sql = new StringBuilder();

            t_sql.Append("SELECT ");

            foreach (FieldInformation t_currFieldinformation in t_tableInformation.Fields)
            {
                t_sql.AppendFormat("{0}.{1} as {1},", t_tableInformation.Name, t_currFieldinformation.Name);
            }

            t_sql.Remove(t_sql.Length - 1, 1);

            if (!String.IsNullOrEmpty(p_sqlFrom))
            t_sql.AppendFormat(@" FROM {0} ", p_sqlFrom);

            if (String.IsNullOrEmpty(p_sqlWhere))
                t_sql.AppendFormat(@" WHERE {0}", p_sqlWhere);

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql.ToString();

            IDataReader t_reader = cmd.ExecuteReader();
            T t_genericObject = default(T);
            if (t_reader.Read())
            {
                t_genericObject = new T();
                foreach (FieldInformation t_currFieldinformation in t_tableInformation.Fields)
                {
                    if (t_reader[t_currFieldinformation.Name] != DBNull.Value)
                    {
                        t_genericObject.GetType().
                            GetProperty(t_currFieldinformation.PropertyName).
                            SetValue(t_genericObject, t_reader[t_currFieldinformation.Name], null);
                    }
                }
            }

            t_reader.Close();

            return t_genericObject;
        }

        #endregion Find

        #region List

        public List<T> ExecuteList<T>(IDbTransaction p_transaction)
            where T:new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateSelectForList();

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDataReader t_reader = cmd.ExecuteReader();
            try
            {
                List<T> t_results = new List<T>();
                while (t_reader.Read())
                {
                    T t_genericObject = new T();

                    this.PopulateEntity(t_reader, t_tableInformation, t_genericObject);

                    t_results.Add(t_genericObject);
                }

                return t_results;     
            }
            finally
            {
                t_reader.Close();
            }
        }

        public List<T> ExecuteList<T>(IDbTransaction p_transaction, string p_sqlWhere)
            where T : new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateSelectByWhere(p_sqlWhere);

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDataReader t_reader = cmd.ExecuteReader();
            try
            {
                List<T> t_results = new List<T>();
                while (t_reader.Read())
                {
                    T t_genericObject = new T();

                    this.PopulateEntity(t_reader, t_tableInformation, t_genericObject);

                    t_results.Add(t_genericObject);
                }

                return t_results;
            }
            finally
            {
                t_reader.Close();
            }
        }

        #endregion List

        #region Delete

        public void ExecuteDeleteCommand(IDbTransaction p_transaction, object p_object)
        {
            TableInformation t_tableInformation = this.GetTableInformations(p_object.GetType());
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(this.m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateDeleteByPrimaryKey();

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;

            IDbDataParameter t_param = cmd.CreateParameter();
            t_param.ParameterName = t_sqlHelper.CreateParameterName(t_tableInformation.PrimaryKey.Name);
            t_param.Value = p_object.GetType().GetProperty(t_tableInformation.PrimaryKey.PropertyName).GetValue(p_object, null);
            cmd.Parameters.Add(t_param);

            cmd.ExecuteNonQuery();
        }

        public void ExecuteDeleteCommand<T>(IDbTransaction p_transaction, string p_sqlWhere)
        where T:new()
        {
            TableInformation t_tableInformation = this.GetTableInformations(typeof(T));
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateDeleteByWhere(p_sqlWhere);

            IDbCommand cmd = p_transaction.Connection.CreateCommand();
            cmd.Transaction = p_transaction;
            cmd.CommandText = t_sql;
            
            cmd.ExecuteNonQuery();
        }

        #endregion Delete

        #region Insert

        public void ExecuteInsertCommand(IDbTransaction p_transaction, object p_object)
        {
            TableInformation t_tableInformation = this.GetTableInformations(p_object.GetType());
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateInsert(p_object);

            IDbCommand t_cmd = p_transaction.Connection.CreateCommand();
            foreach (FieldInformation t_currFieldinformation in t_tableInformation.Fields)
            {
                if (t_currFieldinformation.IsPrimaryKey)
                    continue;

                IDbDataParameter t_param = t_cmd.CreateParameter();
                t_param.ParameterName = t_sqlHelper.CreateParameterName(t_currFieldinformation.Name);
                t_param.Value = p_object.GetType().GetProperty(t_currFieldinformation.PropertyName).GetValue(p_object, null);
                t_cmd.Parameters.Add(t_param);
            }

            t_cmd.CommandText = t_sql.ToString();
            t_cmd.Transaction = p_transaction;
            t_cmd.ExecuteNonQuery();

            IDbCommand t_cmdIDRetriever = p_transaction.Connection.CreateCommand();
            t_cmdIDRetriever.Transaction = p_transaction;
            t_cmdIDRetriever.CommandText = t_sqlHelper.CreateGetLastInsertId(t_tableInformation.PrimaryKey.Name);

            object t_outParam = t_cmdIDRetriever.ExecuteScalar();
            p_object.GetType().GetProperty(t_tableInformation.PrimaryKey.PropertyName).SetValue(p_object,Convert.ToInt32(t_outParam), null);
        }

        #endregion Insert

        #region Update

        public void ExecuteUpdateCommand(IDbTransaction p_transaction, object p_object)
        {
            TableInformation t_tableInformation = this.GetTableInformations(p_object.GetType());
            SqlCommandHelper t_sqlHelper = new SqlCommandHelper(m_providerConfiguration, t_tableInformation);

            string t_sql = t_sqlHelper.CreateUpdate(p_object);

            IDbCommand t_cmd = p_transaction.Connection.CreateCommand();
            foreach (FieldInformation t_currFieldinformation in t_tableInformation.Fields)
            {
                if (t_currFieldinformation.IsPrimaryKey)
                    continue;

                IDbDataParameter t_param = t_cmd.CreateParameter();
                t_param.ParameterName = t_sqlHelper.CreateParameterName(t_currFieldinformation.Name);
                t_param.Value = p_object.GetType().GetProperty(t_currFieldinformation.PropertyName).GetValue(p_object, null);
                t_cmd.Parameters.Add(t_param);
            }

            IDbDataParameter t_paramPK = t_cmd.CreateParameter();
            t_paramPK.ParameterName = t_sqlHelper.CreateParameterName(t_tableInformation.PrimaryKey.Name);
            t_paramPK.Value = p_object.GetType().GetProperty(t_tableInformation.PrimaryKey.PropertyName).GetValue(p_object, null);
            t_cmd.Parameters.Add(t_paramPK);

            t_cmd.CommandText = t_sql.ToString();
            t_cmd.Transaction = p_transaction;
            t_cmd.ExecuteNonQuery();
        }

        #endregion Update

        #endregion Public methods
    }
}