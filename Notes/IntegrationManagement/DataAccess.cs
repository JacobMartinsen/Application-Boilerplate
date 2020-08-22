using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Notes.Models;
using System.Reflection;


namespace IntegrationManagement
{
    public class DataAccess<T>
    {
        private static string _connectionString;

        public DataAccess(string connection)
        {
            if (connection != null || !String.Empty.Equals(connection))
                 _connectionString = connection; 
        }

        public bool Save(iMigratable<T> obj)
        {
            string queryString = "";
            List<Field> fields = GetFields(obj);
            Table table = GetTable(obj);
            Field pkField = GetPKField(fields, table.PRIMARYKEY);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                switch (obj.saveType)
                {
                    case SaveType.CREATE: queryString = QueryBuilder.Insert(table, fields) + QueryBuilder.Endquery(); break;
                    case SaveType.UPDATE: queryString = QueryBuilder.Update(table, fields) + QueryBuilder.Endquery(); break;
                }
                SqlCommand command = new SqlCommand(queryString, connection);
                foreach (var field in fields)
                {
                    string fieldValue = GetFieldValue(obj, field);
                    if (field.ENCRYPT && DataType.VARCHAR.Equals(field.DATATYPE))
                    {
                        fieldValue = EncryptionManagement.Encrypt(fieldValue);
                    }
                    QueryBuilder.ApplyParameter(command, field, fieldValue);
                }
                command.Connection.Open();
                if (command.ExecuteNonQuery() >= 1)
                    return true;
            }
            return false;
        }

        public Boolean Delete(iMigratable<T> obj)
        {
            List<Field> fields = GetFields(obj);
            Table table = GetTable(obj);
            Field pkField = GetPKField(fields, table.PRIMARYKEY);
            string fieldValue = GetFieldValue(obj, pkField);
            Comparison deleteComparison = new Comparison(pkField, ComparisonOperator.EQUALTO, fieldValue);
            return DeleteOnCondition(obj, table, deleteComparison);
        }
        /// <summary>
        /// Retrieves the table name for <paramref name="obj"/> with a Table attribute 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Table GetTable(Object obj)
        {
            return obj.GetType().GetCustomAttributes(typeof(Table), true).Where(r => r != null).FirstOrDefault() as Table;
        }
        /// <summary>
        /// Gets the fields for <paramref name="obj"/> which have a Field attribute 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<Field> GetFields(Object obj)
        {
            return obj.GetType().GetFields().Select(r => r.GetCustomAttributes(typeof(Field), true).FirstOrDefault() as Field).Where(r => r != null).ToList<Field>();
        }
        public Field GetFieldByName(Object obj, string fieldName)
        {
            return GetFields(obj).Where(r => r.COLUMN.Equals(fieldName)).FirstOrDefault<Field>();
        }
        public string GetFieldValue(Object obj, Field field)
        {
            return obj.GetType().GetField(field.COLUMN).GetValue(obj).ToString();
        }
        public Boolean SetFieldValue(Object obj, Field field, object value)
        {

            try
            {
                if (field.ENCRYPT && DataType.VARCHAR.Equals(field.DATATYPE))
                {
                    value = EncryptionManagement.Decrypt(value as string);
                }
                obj.GetType().GetField(field.COLUMN).SetValue(obj, value);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public Object CreateInstance(Object obj)
        {
            return Activator.CreateInstance(obj.GetType());
        }

        public Field GetPKField(IEnumerable<Field> fields, string PKFieldName)
        {
            return fields.Where(r => PKFieldName.Equals(r.COLUMN)).FirstOrDefault();
        }

        /// <summary>
        ///     Performs a database retreival on <paramref name="obj"/> using the primary key specified on the Table atttribute
        /// </summary>
        /// <param name="obj">Object with Table attribute</param>
        /// <returns></returns>
        public bool LoadSingleRowFromPK(Object obj)
        {
            List<Field> fields = GetFields(obj);
            Table table = GetTable(obj);
            Field pkField = GetPKField(fields, table.PRIMARYKEY);
            return LoadTopEntry(obj, table, fields, pkField) != null;
        }
        public IEnumerable<T> LoadNRowsForColumn(Object obj, int N, string fieldName, string comparison)
        {
            if (N <= 0)
                throw new ArgumentException("Number of Rows must be greater than 0.");
            
            Field field = GetFieldByName(obj, fieldName);
            List<Field> fields = GetFields(obj);
            Table table = GetTable(obj);
            Field pkField = GetPKField(fields, table.PRIMARYKEY);
            return LoadTopEntry(obj, table, fields, pkField, comparison, N) as List<T>;
        }
        /// <summary>
        ///     Performs a database retreival on <paramref name="obj"/> using the Field specfied by <paramref name="field"/>
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <param name="field">Field to perform search on</param>
        /// <returns></returns>
        public bool LoadSingleRowByColumn(Object obj, Field searchField)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(searchField.COLUMN);
            if (fieldInfo == null)
                throw new ArgumentException("Invalid field for object");

            List<Field> fields = GetFields(obj);
            Table table = GetTable(obj);
            return LoadTopEntry(obj, table, fields, searchField) != null;
        }
        private Object LoadTopEntry(Object obj, Table table, IEnumerable<Field> fields, Field searchField)
        {
            string searchFieldValue = GetFieldValue(obj, searchField);
            Comparison selectComparison = new Comparison(searchField, ComparisonOperator.EQUALTO, searchFieldValue);
            string queryString = QueryBuilder.SelectTopNum(table, fields, 1) + QueryBuilder.Where(selectComparison) + QueryBuilder.Endquery();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                QueryBuilder.ApplyParameter(command, selectComparison);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        foreach (var field in fields)
                        {
                            if (!SetFieldValue(obj, field, reader[field.COLUMN]))
                                return null;
                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
                finally
                {
                    reader.Close();
                }
            }

            return obj;
        }
        private IEnumerable<Object> LoadTopEntry(Object obj, Table table, IEnumerable<Field> fields, Field searchField, string comparison, int top)
        {
            if (!ComparisonOperator.ValidOperators.Contains(comparison))
                throw new ArgumentException("Operator string is not valid");          

            string searchFieldValue = GetFieldValue(obj, searchField);
            Comparison selectComparison = new Comparison(searchField, comparison, searchFieldValue);
            string queryString = QueryBuilder.SelectTopNum(table, fields, top) + QueryBuilder.Where(selectComparison) + QueryBuilder.Endquery();
            Type listType = typeof(List<>);
            Type[] typeArgs = { obj.GetType() };
            Type makeme = listType.MakeGenericType(typeArgs);
            List<Object> bulkList = Activator.CreateInstance(makeme) as List<Object>;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                QueryBuilder.ApplyParameter(command, selectComparison);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Object newInst = Activator.CreateInstance(obj.GetType());
                        foreach (var field in fields)
                        {
                            if (!SetFieldValue(newInst, field, reader[field.COLUMN]))
                                return null;
                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
                finally
                {
                    reader.Close();
                }
            }
            return bulkList;
        }

        private Boolean DeleteOnCondition(Object obj, Table table, Comparison deleteComparison)
        {
            string queryString = QueryBuilder.Delete(table) + QueryBuilder.Where(deleteComparison) + QueryBuilder.Endquery();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                QueryBuilder.ApplyParameter(command, deleteComparison);
                try
                {
                    command.Connection.Open();
                    if (command.ExecuteNonQuery() >= 1)
                        return true;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    command.Connection.Close();
                }
            }
            return true;
        }
    }
}
