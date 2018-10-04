using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMapper
{

    /// <summary>
    /// Stores the field informarmation and association
    /// </summary>
    struct FieldInformation
    {
        string m_name;

        /// <summary>
        /// Name where Property will be maped
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        bool m_isPrimaryKey;

        /// <summary>
        /// This Field is a Primary Key?
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return m_isPrimaryKey; }
            set { m_isPrimaryKey = value; }
        }
        string propertyName;

        /// <summary>
        /// Name of the property mapped on this Field
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }
    }
}
