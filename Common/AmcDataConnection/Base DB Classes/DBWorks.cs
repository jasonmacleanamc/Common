using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMCDatabase;
using System.Reflection;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;

namespace AMCDatabase
{
    public class DBWorks 
    {
        private bool Found { get; set; }

        public string[] _sdbValues;

        public AmcDataConnection _dbConn = null;
        public List<String> _ColumnNames = new List<String>();

        public string TableName { get; set; }
        public string KeyIdentifier { get; set; }
        public string KeyValue { get; set; }
        public string Version { get; set; }
        public string ConnectionString { get; set; }
        public int DataBaseType { get; set; }

        public string SecondKeyIdentifier { get; set; }
        public string SecondKeyValue { get; set; }

        private int _nRow = 0;

        public List<string> _errorCodes = new List<string>();

        public DBWorks() 
        {
            Found = false;
        }

        public bool InitializeDatabase()
        {
            if (String.IsNullOrWhiteSpace(KeyIdentifier))
                return false;
            
            _dbConn = new AmcDataConnection()
            {
                ConnectionString = ConnectionString,
                TableName = TableName,
                DataBaseType = DataBaseType,
                KeyIdentifier = KeyIdentifier,
                KeyValue = KeyValue,
                WhereClause = GetWhereSQL()
            };

            //_dbConn.WhereClause = GetWhereSQL() + AddVersionToWhere();

            // ja - connect to a table
            if (!_dbConn.ConnectToDatabase())
                return false;

            AddColumns();
           
            if (_ColumnNames.Count > 0)
                Found = _dbConn.FillTable(_ColumnNames);
            else
                Found = _dbConn.FillTable();

            return true;
        }
        
        // ja - allows for an override to only get / set specific columns
        virtual public void AddColumns() { }
        virtual public string GetWhereSQL()
        {
            return "";
        }

        virtual public string AddVersionToWhere()
        {
            return "";
        }

        public bool KeyFound()
        {
            return Found;
        }

        public List<string> GetErrorCodes()
        {
            return _errorCodes;
        }

        public void AddError(string sError)
        {
            _errorCodes.Add(sError);
        }

//         virtual public string GetWhereSQL()
//         {
//             // ja - create the where statement for "Key" lookup and data population
//             string sFormattedKeyValue = _dbConn.__FormatQuote(KeyValue);
//             string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;
// 
//             return sWhere;
//         }

        public bool MoveNext()
        {
            if (_nRow < _dbConn.TheDataTable.Rows.Count)
            {
                _nRow++;
                return true;
            }

            return false;
        }

        public string GetValue(string sCoumnName)
        {
            string sValue = "";
            
            try
            {
                sValue = _dbConn.TheDataTable.Rows[_nRow][sCoumnName].ToString();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sValue.TrimEnd();
        }

        // ja - to be used if you don't want to bind controls
        public void SetDTValue(string sCoumnName, string sNewValue)
        {
           try
           {
               _dbConn.TheDataTable.Rows[0][sCoumnName] = sNewValue;
           }
           catch (System.Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
            
        }

        public bool UpdateTable(string sAndWhere = null)
        {
            Dictionary<string, string> SetPairs = new Dictionary<string, string>();

            foreach (var colName in _ColumnNames)
            {
                SetPairs.Add(colName, GetValue(colName));
            }

            UpdateTables update = new UpdateTables(_dbConn, SetPairs);

            if (!string.IsNullOrEmpty(sAndWhere))
                update.AddAndWhere(sAndWhere);

            return update.PerformUpdate();
        }

        public bool UpdateTable(Dictionary<string, string> SetPairs)
        {
            UpdateTables update = new UpdateTables(_dbConn, SetPairs);

            return update.PerformUpdate();
        }

        public bool UpdateTableOnlyChanged(string[] sColNames, string[] sValues)
        {
            Dictionary<string, string> SetPairs = new Dictionary<string, string>();

            int nCounter = 0;
            foreach (var colName in sColNames)
            {
                SetPairs.Add(colName, sValues[nCounter]);
                nCounter++;
            }

            UpdateTables update = new UpdateTables(_dbConn, SetPairs);

            return update.PerformUpdate();
        }

        public DataTable GetDataTable()
        {
            return _dbConn.TheDataTable;
        }

        public bool UpdateBlob(Dictionary<string, string> SetPairs)
        {
            UpdateTables update = new UpdateTables(_dbConn, SetPairs);

            return update.UpdateBlob();
        }        
    }
}
