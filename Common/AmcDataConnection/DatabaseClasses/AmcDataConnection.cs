using System;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace AMCDatabase
{

    public class AmcDataConnection : AMCDatabase
    {
        public string ConnectionString { get; set; }
        public string WhereClause { get; set; }

        public DataTable TheDataTable { get; set; }
     
        private OleDbConnection _dbConnection;
        private OdbcConnection _dbOBDCConnection;
        private OleDbDataAdapter _dataAdapter;

        public AmcDataConnection()
        {
        }

        public bool ConnectToDatabase()
        {
#if _OLE
            return ConnectToOleDatabase();
#elif _ODBC
            return ConnectToODBCDatabase();
#endif          
        }

        public bool ConnectToOleDatabase()
        {
            bool bRet = false;

            // ja - use the Connection string property
            _dbConnection = new OleDbConnection(ConnectionString);

            _dbConnection.Open();

            bRet = true;
           
            // ja - for VFP test connection
            if (DataBaseType == (int)dbType.vfp)
            {
                try
                {
                    OleDbCommand cmdStartUp = new OleDbCommand("set null off", _dbConnection);
                    cmdStartUp.ExecuteNonQuery();
                }
                catch (System.Exception)
                {                 
                    throw new System.ArgumentException("Test DB Connection Failed", "AMCDatabase");                
                }
            }

            return bRet;
        }

        public bool ConnectToODBCDatabase()
        {
            bool bRet = false;

            // ja - use the Connection string property
            _dbOBDCConnection = new OdbcConnection(ConnectionString);            

            _dbOBDCConnection.Open();

            bRet = true;
            
            // ja - for VFP test connection
            if (DataBaseType == (int)dbType.vfp)
            {
                try
                {
                    OdbcCommand cmdStartUp = new OdbcCommand("set null off", _dbOBDCConnection);
                    cmdStartUp.ExecuteNonQuery();
                }
                catch (System.Exception)
                {
                    throw new System.ArgumentException("Test DB Connection Failed", "AMCDatabase");
                }
            }

            return bRet;
        }
        
        public OleDbConnection GetOleConn()
        {
            // ja - test for valid connection
            if (_dbConnection.State == ConnectionState.Closed)
                throw new System.ArgumentException("DB Connection is Closed", "AMCDatabase");

            return _dbConnection;
        }

        public OdbcConnection GetODBCConn()
        {
            // ja - test for valid connection
            if (_dbOBDCConnection.State == ConnectionState.Closed)
                throw new System.ArgumentException("DB Connection is Closed", "AMCDatabase");

            return _dbOBDCConnection;
        }

        public OleDbDataAdapter GetAdapter()
        {
            return _dataAdapter;
        }

        public string GetDBName()
        {
            return TableName;
        }        

        public bool FillTable(List<string> sColumnNames)
        {
            string sColumns = "";
            int nCounter = 0;
            int nTotal = sColumnNames.Count;

            foreach (var col in sColumnNames)
            {
                sColumns += col;

                if (++nCounter != nTotal)
                    sColumns += ", ";
            }

            string sSelectSql = "select " + sColumns;
            sSelectSql += " from " + TableName; 

            if (WhereClause != "ALL")
                sSelectSql += " where " + WhereClause;
#if _OLE
            return PopulateOleDataSet(sSelectSql);
#elif _ODBC
            return PopulateODBCDataSet(sSelectSql);
#endif     
            
        }

        public bool FillTable()
        {
            string sSelectSql = "select *";
            sSelectSql += " from " + TableName;
            
            if (WhereClause != "ALL")
                sSelectSql += " where " + WhereClause;
#if _OLE
            return PopulateOleDataSet(sSelectSql);
#elif _ODBC
            return PopulateODBCDataSet(sSelectSql);
#endif   
        }

        private bool PopulateOleDataSet(string sSQL)
        {
            TheDataTable = new DataTable();
            _dataAdapter = new OleDbDataAdapter(sSQL, GetOleConn());

            try
            {
                _dataAdapter.Fill(TheDataTable);                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

//             try
//             {
//                 string sValue = TheDataTable.Rows[0][0].ToString();
//             }
//             catch (IndexOutOfRangeException)
//             {
//                 int y = 0;
//             }

            if (TheDataTable.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private bool PopulateODBCDataSet(string sSQL)
        {
            TheDataTable = new DataTable();
            OdbcDataAdapter dataAdapter = new OdbcDataAdapter(sSQL, GetODBCConn());

            try
            {
                dataAdapter.Fill(TheDataTable);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            if (TheDataTable.Rows.Count > 0)
                return true;
            else
                return false;
        }

    }
}
