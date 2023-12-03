using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [DataContract]
    public class Database
    {

        [DataMember]
        public string DBName;
        [DataMember]
        public List<Table> DBTablesList;

        public Database(string dbname)
        {
            DBName = dbname;
            DBTablesList = new List<Table>();
        }
    }
}
