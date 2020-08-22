using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    public class Comparison
    {
        private string _operator = ComparisonOperator.EQUALTO;
        private Field _field { get; set; }
        private string _value1 = String.Empty;
        private string _value2 = String.Empty;
        private string _quoteableValue = String.Empty;
        private string _parameterName = String.Empty;

        public string ParameterName { 
            get
            {
                return _parameterName;
            }
            private set
            {
                _parameterName = value;
            }
        }
        public string Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                if (ComparisonOperator.ValidOperators.Contains(value.Trim()))
                    _operator = value;
                else
                    throw new ArgumentException("Argument must be a correct operator");
            }
        }
        public string Value1 {
            get 
            {
               return _value1;
            }
            set 
            {
                _value1 = value; 
            } 
        }

        public string Value2
        {
            get
            {
                return _value2;
            }
            set
            {
                _value2 = value;
            }
        }

        public string QuoteableValue
        {
            get
            {
                return Datatype == DataType.VARCHAR ? $" QUOTENAME('{Value1}') " : Value1;
            }
            set
            {
                _value1 = value;
            }
        }

        public string Column { 
            get 
            {
                if (_field == null)
                    throw new ArgumentException("No Field present");
                return _field.COLUMN;
            }
            set 
            {
                throw new ArgumentException("Value is not to be set directly");

            }
        }

        public DataType Datatype
        {
            get
            {
                if (_field == null)
                    throw new ArgumentException("No Field present");
                return _field.DATATYPE;
            }
            set
            {
                throw new ArgumentException("Value is not to be set directly");
            }
        }

        public Comparison(Field field, string operatorString, string comparisonValue)
        {
            _field = field;
            Operator = operatorString;
            Value1 = comparisonValue;
            ParameterName = QueryBuilder.ParameterDelimeter + field.COLUMN; 
        }
        /// <summary>
        /// Comparison for comparators with two arguments 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operatorString"></param>
        /// <param name="comparisonValue1"></param>
        /// <param name="comparisonValue2"></param>
        public Comparison(Field field, string operatorString, string comparisonValue1, string comparisonValue2)
        {
            _field = field;
            Operator = operatorString;
            Value1 = comparisonValue1;
            ParameterName = QueryBuilder.ParameterDelimeter + field.COLUMN;
        }

        public string GetComparisonString()
        {
            if (ComparisonOperator.BETWEEN.Equals(Operator))
                return QueryBuilder.And($" {ParameterName} {ComparisonOperator.BETWEEN} {Value1} ", $" {Value2} ");
            else
                return $" {Column} {Operator} {ParameterName} ";
        }


    }
}
