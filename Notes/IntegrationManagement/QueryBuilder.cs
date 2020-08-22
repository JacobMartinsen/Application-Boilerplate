using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace IntegrationManagement
{
    public class QueryBuilder
    {
        public static string ParameterDelimeter = "@";

        public static string Select(Table table, IEnumerable<Field> fields)
        {
            return $" SELECT {String.Join(",",fields.Select(f => $"{table.TABLE}.{f.COLUMN}").ToArray())} FROM [{table.DBO}].[dbo].{table.TABLE} ";
        }

        public static string Insert(Table table, IEnumerable<Field> fields)
        {
            return $"INSERT INTO [{table.DBO}].[dbo].{table.TABLE} ({String.Join(",", fields.Select(f => $"{f.COLUMN}"))}) VALUES ({String.Join(",", fields.Select(f => f.PARAMETER))}) ";
        }

        public static string Update(Table table, IEnumerable<Field> fields)
        {
            return $"UPDATE [{table.DBO}].[dbo].{table.TABLE} SET {String.Join(",", fields.Select(f => $"{f.COLUMN} = {f.PARAMETER}"))} ";
        }

        public static string SelectTopNum(Table table, IEnumerable<Field> fields, int topNum)
        {
            return $" SELECT TOP {topNum} {String.Join(",", fields.Select(f => $"{table.TABLE}.{f.COLUMN}").ToArray())} FROM [{table.DBO}].[dbo].{table.TABLE} ";
        }
        public static string SelectTopPercent(Table table, IEnumerable<Field> fields, int topNum)
        {
            return $" SELECT TOP {topNum} PERCENT {String.Join(",", fields.Select(f => $"{table.TABLE}.{f.COLUMN}").ToArray())} FROM [{table.DBO}].[dbo].{table.TABLE} ";
        }
        public static string And(params string[] clauses)
        {
            return string.Join(" AND ", clauses);
        }
        public static string Or(params string[] clauses)
        {
            return string.Join(" AND ", clauses);
        }
        /// <summary>
        /// TODO: CREATE CONSTRAINT CLASS AND IMPLEMENT WHERE RELATIONS FOR =, !=, >... 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="constraints"></param>
        /// <returns>String query</returns>
        public static string Where(IEnumerable<Comparison> constraints)
        {
            return $" WHERE {String.Join(",", constraints.Select(r => $"{r.Column} {r.Operator} {r.ParameterName}").ToArray())} ";
        }

        /// <summary>
        /// TODO: CREATE CONSTRAINT CLASS AND IMPLEMENT WHERE RELATIONS FOR =, !=, >... 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="constraint"></param>
        /// <returns>String query</returns>
        public static string Where(Comparison constraint)
        {
            return $" WHERE {constraint.Column} {constraint.Operator} {constraint.ParameterName} ";
        }

        /// <summary>
        /// Creates an update SQL query
        /// </summary>
        /// <param name="table"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Update(Table table, IEnumerable<Comparison> comparisons)
        {
            return $" UPDATE [{table.DBO}].{table.TABLE} SET {String.Join(",", comparisons.Select(r => $"{r.Column} {r.Operator} {r.ParameterName}").ToArray())} ";
        }

        /// <summary>
        /// Creates a delete SQL query
        /// </summary>
        /// <param name="table">Table object to delete from</param>
        /// <returns>string query</returns>
        public static string Delete(Table table)
        {
            return $" DELETE FROM [{table.DBO}].[dbo].{table.TABLE} ";
        }

        public static string Endquery()
        {
            return ";";
        }

        public static void ApplyParameters(SqlCommand command, IEnumerable<Comparison> comparisons)
        {
            foreach (var comp in comparisons)
            {
                ApplyParameter(command, comp);
            }
        }
        public static void ApplyParameter(SqlCommand command, Comparison comparison)
        {
                command.Parameters.AddWithValue(comparison.ParameterName, comparison.Value1);
        }
        public static void ApplyParameter(SqlCommand command, Field field, string value)
        {
            command.Parameters.AddWithValue(field.PARAMETER, value);
        }


    }
}
