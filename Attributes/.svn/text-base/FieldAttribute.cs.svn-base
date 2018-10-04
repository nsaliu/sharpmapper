using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMapper
{
    /// <summary>
    /// Indicates which DB Field has to be mapped.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        private string m_fieldName;
        public string FieldName
        {
            get { return m_fieldName; }
            set { m_fieldName = value; }
        }

        bool m_isPrimaryKey = false;
        public bool IsPrimaryKey
        {
            get { return m_isPrimaryKey; }
            set { m_isPrimaryKey = value; }
        }

        public FieldAttribute(string p_fieldName)
        {
            this.m_fieldName = p_fieldName;
        }

        public FieldAttribute(string p_fieldName, bool p_primaryKey)
        {
            this.m_fieldName = p_fieldName;
            this.m_isPrimaryKey = p_primaryKey;
        }
    }
}