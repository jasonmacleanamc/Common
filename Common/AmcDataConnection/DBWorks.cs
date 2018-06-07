using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMCDatabase;
using System.Reflection;

namespace AMCDatabase
{
    public class DBWorks : TableInterface
    {
        // TODO: ja 
        private bool _bIsFound = false;

        public string[] _sdbValues;

        public AmcDataConnection _dbConn = null;
        public List<String> _ColumnNames = new List<String>();

        public string TableName { get; set; }
        public string KeyIdentifier { get; set; }
        public string KeyValue { get; set; }
        public string Version { get; set; }
        public string ConnectionString { get; set; }
        public int DataBaseType { get; set; }

        public DBWorks() 
        {

        }

        // TODO: add support for version
        public DBWorks(string sTableName, string sKeyValue, string sKeyIdentifier)
        {
            KeyValue = sKeyValue;
            KeyIdentifier = sKeyIdentifier;
            TableName = sTableName;
        }

        public bool InitializeDatabase()
        {
            // ja - use reflection to get values from the base class
            //DataBaseType = Convert.ToInt32(this.GetType().GetField("nDATABASE_TYPE", BindingFlags.Public | BindingFlags.Instance).GetValue(this).ToString());
            //ConnectionString = this.GetType().GetField("CONNECTION_STRING", BindingFlags.Public | BindingFlags.Instance).GetValue(this).ToString();

            // ja - init the connection object
            //_dbConn = new AmcDataConnection((dbType)DataBaseType);

            _dbConn = new AmcDataConnection()
            {
                ConnectionString = ConnectionString,
                TableName = TableName,
                DataBaseType = DataBaseType
            };

            // ja - connect to a table
            if (!_dbConn.__ConnectToDB())
                return false;

            // ja - call local function to populate the data in the string global array
            // TODO: ja - Move to object
            _sdbValues = ReadData();


            if (String.IsNullOrEmpty(_sdbValues[0]))
                return false;

            _bIsFound = true;

            return true;
        }
        
        virtual public void AddColums() {}

        virtual public void UpdateTable() {}

        public bool KeyFound()
        {
            return _bIsFound;
        }

        public string[] ReadData()
        {
            // ja - get the column names from the base class and put into global object
            // TODO: get rid of global??
            AddColums();

            // ja - create a new string array
            string[] sColums = new string[_ColumnNames.Count];

            // ja - loop trough the columns and add to the array
            int i = 0;
            foreach (var line in _ColumnNames)
            {
                sColums[i] = line.Trim();
                i++;
            }

            // ja - create the sql select statement and populate from the db
            string[] sValues = _dbConn.__GetSelectData(sColums, GetWhereSQL());

            return sValues;
        }

        public string GetWhereSQL()
        {
            // ja - create the where statement for "Key" lookup and data population
            string sFormattedKeyValue = _dbConn.__FormatQuote(KeyValue);
            string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;

            return sWhere;
        }

        public string GetValue(string sCoumnName)
        {
            // ja - get the position for a column name 
            int i = GetColumnPosition(sCoumnName);

            // ja - return the data at that position
            if (i < _ColumnNames.Count)
                return _sdbValues[i];

            return "";

        }

        private int GetColumnPosition(string sCoumnName)
        {
            int i = 0;
            foreach (var col in _ColumnNames)
            {
                // ja - loop trough until we find a match
                if (col == sCoumnName.Trim())
                    break;
                i++;
            }

            return i;
        }

        public void SetValue(string sCoumnName, string sNewValue)
        {
            // ja - get position
            int i = GetColumnPosition(sCoumnName);

            if (i < _ColumnNames.Count)
            {
                // ja - add the value to the update sql 
                _sdbValues[i] = sNewValue;
                _dbConn.addUpdateSQLValues(sCoumnName, sNewValue);
            }
        }
    }
}
