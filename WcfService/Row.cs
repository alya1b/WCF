using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [DataContract]
    public class Row
    {
        [DataMember]
        public List<string> RowValuesList;

        public Row()
        {
            RowValuesList = new List<string>();
        }
    }
}
