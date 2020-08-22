using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    public class ComparisonOperator
    {
        public const string EQUALTO = "=";
        public const string GREATERTHAN = ">";
        public const string GREATERTHANEQUALTO = ">=";
        public const string LESSTHAN = "<";
        public const string LESSTHANEQALTO = "<=";
        public const string LIKE = "%";
        public const string NOTEQUALS = "<>";
        public const string BETWEEN = " between ";
        public static readonly string[] ValidOperators = new string[]{EQUALTO, GREATERTHAN, GREATERTHANEQUALTO, LESSTHAN, LESSTHANEQALTO, LIKE, NOTEQUALS, BETWEEN.Trim()};

    }
}
