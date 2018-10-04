using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMapper
{
    /// <summary>
    /// Allow Mapping Between classes and structs over DB.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct,
                      AllowMultiple = false, Inherited = false)]  // multiuse attribute
    public class TableAttribute : Attribute
    {
        private string m_tableName;
        public string TableName
        {
            get { return m_tableName; }
            set { m_tableName = value; }
        }

        public TableAttribute(string p_tableName)
        {
            this.m_tableName = p_tableName;
        }
    }
}