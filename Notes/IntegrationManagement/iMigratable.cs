using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationManagement
{
    public class iMigratable<T>
    {
        private DataAccess<T> _dataAccess;
        string _connectionString = "Data Source=jacob-g5\\sqlexpress;Initial Catalog=NotesDB;Persist Security Info=True;User ID=TestUser;Password=TestUser";
        private IEnumerable<T> _bulkLoadList = null;
        public IEnumerable<T> BulkLoadList {
            get
            {
                return _bulkLoadList;
            }
            protected set 
            {
                _bulkLoadList = value;
            }
        }

        public iMigratable()
        {
            _dataAccess = new DataAccess<T>(_connectionString);
        }
        public virtual Boolean Save() { return _dataAccess.Save(this); }
        public virtual Boolean Load() { return _dataAccess.LoadSingleRowFromPK(this); }
        public virtual Boolean Delete() { return _dataAccess.Delete(this); }
        public virtual Boolean BulkLoad(int N, string fieldName, string comparison) { return (BulkLoadList = _dataAccess.LoadNRowsForColumn(this, N, fieldName, comparison)) != null; }
        private SaveType _saveType;
        public SaveType saveType
        {
            get
            {
                return _saveType;
            }
            set
            {
                if (value != SaveType.CREATE || value != SaveType.UPDATE)
                    throw new ArgumentException("Invalid save type");
                _saveType = value;
            }
        }
    }
}
