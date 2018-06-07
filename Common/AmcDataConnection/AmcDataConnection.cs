using System;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;

namespace AMCDatabase
{
    public enum dbType
    {
        vfp,
        sqlProductionManualLabels,
        sqlEcoTracker
    }

    public enum dbTable
    {
        beta,
        current,
        proto,
        history,
        not_found
    }

    public class AmcDataConnection
    {
        // TODO: ja - change for SQL vs PM
        public static string QUOTE = "\"";
        public static string SINGLE_QUOTE = "\'";

        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public int DataBaseType { get; set; }
        
        // ja - visual fox pro connection settings
        private const string m_sVFPConnProviderString = "Provider=VFPOLEDB.1;";
        
//#if DEBUG        
//        private string _sVFPConnDataSourceString = "Data Source=N:\\Prodman.dev\\DATABASE\\";
//#else
        private string _sVFPConnDataSourceString = "Data Source=N:\\Prodman.vfp\\DATABASE\\";
//#endif
        private string _sTableName = "";

        string _sEcoConnectionString = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=VantageRead;Password=0nc3ler;Initial Catalog=ECO_Tracker;Data Source=BUSHMASTER";
        string _sPMLabelsConnectionString ="Provider=SQLOLEDB;Data Source=BUSHMASTER;Initial Catalog=ProductionManualLabels;Integrated security=SSPI;";
        
        // ja - vfp where clause arrays
        private ArrayList _UpdateSQLValuesList = new ArrayList();
        private ArrayList _UpdateSQLWhereList = new ArrayList();

        private OleDbConnection _dbConnection;

        public int _nDatabaseConnectionType;

        public AmcDataConnection() {}

        public AmcDataConnection(object nDatabaseConnectionType, bool bIsTest = false)
        {
            _nDatabaseConnectionType = (int)nDatabaseConnectionType;

            if (bIsTest)
            {
                _sVFPConnDataSourceString = "Data Source=N:\\Prodman.dev\\DATABASE\\";
            } 
        }

        public void setConnectionPath(string sPath)
        {
            // ja - override the connection string to update external tables
            _sVFPConnDataSourceString = "Data Source=" + sPath;
        }

        public bool __ConnectToDB()
        {
            return __ConnectToDatabase();
        }

        public bool __ConnectToDatabase()
        {
            bool bRet = false;

            _dbConnection = new OleDbConnection(ConnectionString);

            OleDbCommand cmdStartUp = new OleDbCommand("set null off", _dbConnection);

            try
            {
                _dbConnection.Open();

                bRet = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }

            if (DataBaseType == (int)dbType.vfp)
            {
                try
                {
                    cmdStartUp.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    return false;
                }
            }

            return bRet;
        }

        public bool connect(string sTableName)
        {
            string sConnectionString = "";

            if (_nDatabaseConnectionType == (int)dbType.vfp)
            {
                // ja - create the connection string
                sConnectionString = m_sVFPConnProviderString + _sVFPConnDataSourceString + sTableName + ";";

                // ja - store the table name for later
                string[] sDbPureName = sTableName.Split('.');

                _sTableName = sDbPureName[0];
            }
            else
            {
                // ja - create the sql connection string
                if (_nDatabaseConnectionType == (int)dbType.sqlProductionManualLabels)
                    sConnectionString = _sPMLabelsConnectionString;
                else if (_nDatabaseConnectionType == (int)dbType.sqlEcoTracker)
                    sConnectionString = _sEcoConnectionString;
                
                _sTableName = sTableName;
            }                       

            return ConnectToDatabase(sConnectionString);
        }

        public OleDbConnection GetConn()
        {
            // ja - TODO: test for valid connection

            return _dbConnection;
        }

        public string GetDBName()
        {
            return TableName;
        }

        public bool ConnectToDatabase(string sConnectionString)
        {
            bool bRet = false;

            _dbConnection = new OleDbConnection(sConnectionString);

            OleDbCommand cmdStartUp = new OleDbCommand("set null off", _dbConnection);

            try
            {
                _dbConnection.Open();

                bRet = true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                return false;
            }

            if (_nDatabaseConnectionType == (int)dbType.vfp)
            {
                try
                {
                    cmdStartUp.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    return false;    	
                }
            }

            return bRet;
        }

        private bool updateTable(string sUpdateSQLString)
        {
            bool bRet = true;

            OleDbCommand SQlUpdate = new OleDbCommand(sUpdateSQLString, GetConn());

            //Thread workerThread = new Thread(this.ExecuteNonQueryThreadTask);

            // ja - Start the worker thread.
            //workerThread.Start(SQlUpdate);

            if (SQlUpdate.ExecuteNonQuery() == -1)
                bRet = false;

            return bRet;
        }

#if not_now
        public OleDbDataReader selectTable(string sSelectSQLString)
        {
            bool bRet = true;

            OleDbCommand cmdSelect = new OleDbCommand(sSelectSQLString, GetConn());

            Thread workerThread = new Thread(this.ExecuteReaderThreadTask);

            // ja - Start the worker thread.
            workerThread.Start(cmdSelect);

            return m_rdr;
        }
#endif

        public bool performUpdate()
        {
            // ja - create the update sql statement dynamically
            string sUpdateSQLString = "update " + _sTableName + " set ";

            // ja - extract the update values
            foreach (Object obj in _UpdateSQLValuesList)
            {
                sUpdateSQLString += obj;
            }

            if (_UpdateSQLWhereList.Count > 0)
                sUpdateSQLString += " where ";

            foreach (Object obj in _UpdateSQLWhereList)
            {
                sUpdateSQLString += obj;

                // ja - TODO: add and logic
            }

            // ja - update the database with the string
            updateTable(sUpdateSQLString);

            _UpdateSQLValuesList.Clear();
            _UpdateSQLWhereList.Clear();

            return true;
        }

        public void addUpdateSQLValues(string sColumnName, string sValue, bool nNeedsQuotes = true)
        {
            string sUpdateString = "";
            string sQuote = QUOTE;

            if (!nNeedsQuotes)
                sQuote = "";

            if (_UpdateSQLValuesList.Count > 0)
                sUpdateString += ", ";

            sUpdateString += sColumnName += " = " + sQuote + sValue + sQuote;

            _UpdateSQLValuesList.Add(sUpdateString);

        }

        public void AddUpdateSQLWhere(string sColumnName, string sValue, bool nNeedsQuotes = true)
        {
            string sWhereString = "";
            string sQuote = QUOTE;

            if (!nNeedsQuotes)
                sQuote = "";

            sWhereString += sColumnName += " = " + sQuote + sValue + sQuote;

            _UpdateSQLWhereList.Add(sWhereString);

        }

        private void ExecuteNonQueryThreadTask(object cmd)
        {
            OleDbCommand dbCmd = (OleDbCommand)cmd;

            try
            {
                dbCmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
            	
            }
        }

        private void ExecuteReaderThreadTask(object cmd)
        {
            OleDbCommand dbCmd = (OleDbCommand)cmd;

            //m_rdr = dbCmd.ExecuteReader();


            // ja - TODO: return error

        }

//         public int DoIt(string s)
//         {
//             int nRet = -1;
//             
//             OleDbCommand SQlUpdate = new OleDbCommand(s, GetConn());
// 
//             try
//             {
//                 nRet = SQlUpdate.ExecuteNonQuery();
//             }
//             catch (System.Exception ex)
//             {            	
//             }
// 
//             return nRet;
//         }

        public List<string> GetSelectData(List<string> sColumns, string sWhere)
        {
            List<string> sResults = new List<string>();

            string sSelectSql = "select ";

            int i = 0;
            int nCount = sColumns.Count;
    
            foreach (string field in sColumns)
            {
                sSelectSql += field;

                if (++i != nCount)
                    sSelectSql += ", ";
            }
            
            sSelectSql += " from " + TableName + " where " + sWhere;

            OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, GetConn());
            OleDbDataReader rdr = null;

            try
            {
                rdr = cmdSelect.ExecuteReader();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);

                return sResults;
            }

            if (rdr.HasRows)
            {

                while (rdr.Read())
                {
                    sResults.Add(rdr.GetString(0));
                }
            }

            rdr.Close();

            return sResults;
        }

        public string[] __GetSelectData(string[] sColumnsArray, string sWhere)
        {

            //GetSelectData(sColumnsArray.Cast.ToList(), sWhere);

            int nItems = sColumnsArray.Length;

            string[] sResults = new string[nItems];

            string sSelectSql = "select ";

            for (int i = 0; i < nItems; i++)
            {
                sSelectSql += sColumnsArray[i];

                if (i != nItems - 1)
                    sSelectSql += ", ";
            }

            sSelectSql += " from " + TableName + " where " + sWhere;

            OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, GetConn());
            OleDbDataReader rdr = null;

            try
            {
                rdr = cmdSelect.ExecuteReader();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);

                return sResults;
            }

            rdr.Read();

            if (rdr.HasRows)
            {
                for (int t = 0; t < sColumnsArray.Length; t++)
                {
                    sResults[t] = rdr[sColumnsArray[t]].ToString();
                }
            }

            return sResults;
        }


        public string[] GetSelectData(string[] sColumnsArray, string sWhere)
        {

            //GetSelectData(sColumnsArray.Cast.ToList(), sWhere);
            
            int nItems = sColumnsArray.Length;

            string[] sResults = new string[nItems];

            string sSelectSql = "select ";

            for (int i = 0; i < nItems; i++)
            {
                sSelectSql += sColumnsArray[i];

                if (i != nItems - 1)
                    sSelectSql += ", ";
            }

            sSelectSql += " from " + GetDBName() + " where " + sWhere;

            OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, GetConn());
            OleDbDataReader rdr = null;

            try
            {
                rdr = cmdSelect.ExecuteReader();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                return sResults;
            }
            
            rdr.Read();

            if (rdr.HasRows)
            {
                for (int t = 0; t < sColumnsArray.Length; t++)
                {
                    sResults[t] = rdr[sColumnsArray[t]].ToString();
                }
            }

            return sResults;
        }

        public string __FormatQuote(string sValue)
        {
            if (DataBaseType == (int)dbType.vfp)
            {
                return AmcDataConnection.QUOTE + sValue + AmcDataConnection.QUOTE;
            }
            else
            {
                return AmcDataConnection.SINGLE_QUOTE + sValue + AmcDataConnection.SINGLE_QUOTE;
            }
        }

        public string FormatQuote(string sValue)
        {
            if (_nDatabaseConnectionType == (int)dbType.vfp)
            {
                return AmcDataConnection.QUOTE + sValue + AmcDataConnection.QUOTE;
            }
            else
            {
                return AmcDataConnection.SINGLE_QUOTE + sValue + AmcDataConnection.SINGLE_QUOTE;
            }
        }
    }
}
