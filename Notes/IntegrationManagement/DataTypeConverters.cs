using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    public class DataTypeConverters
    {

        public static string VARCHAR (string varcharLiteral) => varcharLiteral;
        public static Int32 INT (string varcharLiteral) => Int32.Parse(varcharLiteral);
        public static double FLOAT (string varcharLiteral) => double.Parse(varcharLiteral);
        public static double DOUBLE(string varcharLiteral) => double.Parse(varcharLiteral);
        public static DateTime DATE(string varcharLiteral) => DateTime.Parse(varcharLiteral).Date;
        public static DateTime DATETIME(string varcharLiteral) => DateTime.Parse(varcharLiteral);
        public static Guid UUID(string varcharLiteral) => Guid.Parse(varcharLiteral);
        public static Object ConvertDataType(Field field, Object obj)
        {
            switch (field.DATATYPE)
            {
                case DataType.VARCHAR:
                    return DataTypeConverters.VARCHAR((string)obj);
                case DataType.INT:
                    return DataTypeConverters.INT((string)obj);
                case DataType.FLOAT:
                    return DataTypeConverters.FLOAT((string)obj);
                case DataType.DOUBLE:
                    return DataTypeConverters.DOUBLE((string)obj);
                case DataType.DATE:
                    return DataTypeConverters.DATE((string)obj);
                case DataType.DATETIME:
                    return DataTypeConverters.DATETIME((string)obj);
                case DataType.UUID:
                    return DataTypeConverters.UUID((string)obj);
                default:
                    throw new ArgumentException($"No supported converter exists for field {field.COLUMN}");
            }
        }
    }
}
