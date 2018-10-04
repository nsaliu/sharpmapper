using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMapper
{
    /// <summary>
    /// Stores Type Table Mapping Information
    /// </summary>
    struct TableInformation
    {
        string m_name;

        /// <summary>
        /// Name of the Table 
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        FieldInformation m_primaryKey;

        /// <summary>
        /// Field that maps PrimaryKey
        /// </summary>
        public FieldInformation PrimaryKey
        {
            get { return m_primaryKey; }
            set { m_primaryKey = value; }
        }

        FieldInformation[] m_fields;

        /// <summary>
        /// List of Property Mapped on the table Fields
        /// </summary>
        public FieldInformation[] Fields
        {
            get { return m_fields; }
            set { m_fields = value; }
        }

    }

}
