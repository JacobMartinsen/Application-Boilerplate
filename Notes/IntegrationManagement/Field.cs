using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class Field : System.Attribute 
    {
        public string COLUMN { get; set; }
        public DataType DATATYPE { get; set; }
        public Boolean ENCRYPT { get; set; }
        public readonly string PARAMETER;

        public Field(string col, DataType dtype, Boolean encrypt)
        {
            ENCRYPT = encrypt;
            COLUMN = col;
            DATATYPE = dtype;
            PARAMETER = QueryBuilder.ParameterDelimeter + col;
        }
    }
}
