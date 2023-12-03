using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    public class TypeCharInvl : TypeDB
    {
        public override bool Validation(string value)
        {
            string[] buf = value.Replace(" ", "").Split(',');
            return buf.Length == 2 && buf[0].Length == 1 && buf[1].Length == 1 && char.TryParse(buf[0], out char a) &&
            char.TryParse(buf[1], out char b) && a < b;
        }

    }
}
