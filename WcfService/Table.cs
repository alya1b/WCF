using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [DataContract]
    public class Table
    {
        [DataMember]
        public string TableName;
        [DataMember]
        public List<Column> TableColumnsList;
        [DataMember]
        public List<Row> TableRowsList;

        public Table(string tablename)
        {
            TableName = tablename;
            TableColumnsList = new List<Column>();
            TableRowsList = new List<Row>();
        }
    }
}
