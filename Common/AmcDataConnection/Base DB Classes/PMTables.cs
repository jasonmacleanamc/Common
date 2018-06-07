/* Database Type Class
 * 
 * This class must inherit from DBWorks
 * 
 * The Constructor must assign the DatabaseType and Connection String
 * 
 * Other Methods in this class are specific to the Database type we are connecting to
 * 
 */

using System;
using System.Data.OleDb;

namespace AMCDatabase
{   
    public class PMTables : DBWorks
    {
        public int DATABASE_TYPE = (int)dbType.vfp;
        protected bool TestData {get; set;}
        
        public PMTables()
        {   
            DataBaseType = DATABASE_TYPE;
        }        
    
        virtual public string GetPMConnString()
        {
#if _OLE
            return GetOlePMConnString();
#elif _ODBC 
            return GetODBCPMConnString();
#endif
        }

        virtual public string GetOlePMConnString()
        {
            string sDbPath = "vfp";

            if (TestData)
                sDbPath = "dev";
             
            // ja - create the connection string
            return @"Data Source=\\BOA\Production\Prodman." + sDbPath + @"\DATABASE\" + TableName + ".dbf;" + @"Provider=VFPOLEDB.1;";
        }

        virtual public string GetODBCPMConnString()
        {
            string sDbPath = "vfp";

            if (TestData)
                sDbPath = "dev";

            // ja - create the connection string
            string sConn = "Driver={Microsoft Visual FoxPro Driver};UID=;PWD=;";
            sConn += @"SourceDB=\\BOA\Production\Prodman." + sDbPath + @"\DATABASE\" + TableName + ".dbf;";
            sConn += "SourceType=DBF;Exclusive=No;BackgroundFetch=Yes;Collate=Machine;Null=Yes;Deleted=Yes;";

            return sConn;
        }
                 
        override public string GetWhereSQL()
        {
            // ja - create the where statement for "Key" lookup and data population
            string sFormattedKeyValue = AmcDataConnection.QUOTE + KeyValue + AmcDataConnection.QUOTE; 
            string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;

            sWhere += AddVersionToWhere();                       

            return sWhere;
        }

        override public string AddVersionToWhere()
        {
            string sVersionWhere = "";
            
            // ja - for 2nd part of where statement 
            if (!string.IsNullOrEmpty(Version))
            {
                sVersionWhere = "and Version = " + AmcDataConnection.QUOTE + Version + AmcDataConnection.QUOTE;
            }

            return sVersionWhere;
        }
        
        public static string GetPartNumberTable(string sPartNumber, string sVersion)
        {
            string[] sTables = new string[] { "beta", "current",  "proto", "history" };

            foreach (string sTable in sTables)
            {
                if (FindTable(sTable, sPartNumber, sVersion))
                    return sTable;
            }

            return "";
        }

        private static bool FindTable(string sTablename, string sPartNumber, string sVersion)
        {
            string sConn = @"Data Source=\\BOA\Production\Prodman.vfp\DATABASE\" + sTablename + ".dbf;" + @"Provider=VFPOLEDB.1;";
            string sFormattedPartNum = AmcDataConnection.QUOTE + sPartNumber + AmcDataConnection.QUOTE;
            string sFormattedVersion = AmcDataConnection.QUOTE + sVersion + AmcDataConnection.QUOTE;
            
            AmcDataConnection vfpTempConn = new AmcDataConnection()
            {
                DataBaseType = (int)dbType.vfp,
                TableName = sTablename,
                ConnectionString = sConn
            };
            
            if (vfpTempConn.ConnectToDatabase())
            {
                string sSelectSql = "select partnumber from " + vfpTempConn.TableName + ".dbf where Partnumber" + " = " + sFormattedPartNum + " and Version = " + sFormattedVersion;
                OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, vfpTempConn.GetOleConn());
                OleDbDataReader rdr = cmdSelect.ExecuteReader();

                if (rdr.HasRows)
                    return true;
            }

            return false;
        }

        public static string GetLatestVersion(string sPartNumber)
        {
            string sVersion = "0.01";

            // ja - current
            if (!FindLasestVersionForTable(sPartNumber, "Current", ref sVersion))
            {                
                // ja - beta
                if (!FindLasestVersionForTable(sPartNumber, "Beta", ref sVersion))
                {
                    // ja - history
                    if (!FindLasestVersionForTable(sPartNumber, "History", ref sVersion))
                        return "0.00";
                }
            }                       

            return sVersion;
        }

        private static bool FindLasestVersionForTable(string sPartNumber, string sTableName, ref string sVersion)
        {
            string sConn = @"Data Source=\\BOA\Production\Prodman.vfp\DATABASE\" + sTableName + ".dbf;" + @"Provider=VFPOLEDB.1;";
            string sFormattedPartNum = AmcDataConnection.QUOTE + sPartNumber + AmcDataConnection.QUOTE;
            
            AmcDataConnection vfpTempConn = new AmcDataConnection()
            {
                DataBaseType = (int)dbType.vfp,
                TableName = sTableName,
                ConnectionString = sConn
            };

            if (vfpTempConn.ConnectToDatabase())
            {
                string sSelectSql = "select top 1 partnumber, version from " + vfpTempConn.TableName + " where Partnumber" + " = " + sFormattedPartNum + " ORDER BY version descending"; 
                OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, vfpTempConn.GetOleConn());
                OleDbDataReader rdr = cmdSelect.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Read();

                    sVersion = rdr["version"].ToString();
            
                    return true;
                }                    
            }

            return false;
        }
    }
}
