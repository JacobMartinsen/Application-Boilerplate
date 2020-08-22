using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    public class Table : System.Attribute
    {
        public string TABLE { get; set; }
        public string DBO { get; set; }
        public string PRIMARYKEY { get; set; }

        public Table(string table, string primaryKey, string dbo)
        {
            TABLE = table;
            DBO = dbo;
            PRIMARYKEY = primaryKey;
        }
    }
}
