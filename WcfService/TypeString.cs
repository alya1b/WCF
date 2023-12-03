using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    class TypeString : TypeDB
    {
        public override bool Validation(string value)
        {
            return true;
        }
    }
}
