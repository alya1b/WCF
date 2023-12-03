using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    public class TypeStringInvl : TypeDB
    {
        public override bool Validation(string value)
        {
            // Розділити рядок на дві частини за допомогою символа " - "
            string[] parts = value.Split(new string[] { " - " }, StringSplitOptions.None);

            if (parts.Length != 2)
            {
                return false; // Неправильний формат (повинен бути роздільник " - ")
            }

            string value1 = parts[0].Trim();
            string value2 = parts[1].Trim();

            // Перевірити, чи value1 менше value2
            if (string.Compare(value1, value2) < 0)
            {
                return true;
            }

            return false;
        }
    }
}
