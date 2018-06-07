using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AMCDatabase
{
    // ja - this class will auto populate from a VFP table for a given single key
    public class PMTables : DBWorks
    {
        public int nDATABASE_TYPE = (int)dbType.vfp;
        public string CONNECTION_STRING1 = @"Data Source=N:\Prodman.dev\DATABASE\";
        public string CONNECTION_STRING2 = @"Provider=VFPOLEDB.1;"; 

        #region Constructors
        public PMTables()
        {   
            DataBaseType = nDATABASE_TYPE;
        }

        
        
        public PMTables(string sTableName, string sKeyValue, string sKeyIdentifier, string sVersion, bool bIsTest = false) 
             //: base(sTableName, sKeyValue, sKeyIdentifier)
         {
             KeyValue = sKeyValue;
             KeyIdentifier = sKeyIdentifier;
             Version = sVersion;
             TableName = sTableName;
             ConnectionString = CONNECTION_STRING1;
             DataBaseType = nDATABASE_TYPE;
         }

         public PMTables(string sTableName, string sKeyValue, string sKeyIdentifier, bool bIsTest = false)
             //: base(sTableName, sKeyValue, sKeyIdentifier)
         {
             KeyValue = sKeyValue;
             KeyIdentifier = sKeyIdentifier;
             //Version = sVersion;
             TableName = sTableName;
             ConnectionString = CONNECTION_STRING1;
             DataBaseType = nDATABASE_TYPE;   
         }

        // TODO: ja - add test
         public string GetPMConnString()
         {
             // ja - create the connection string
             return CONNECTION_STRING1 + TableName + ".dbf;" + CONNECTION_STRING2;
         }
                
        #endregion
#if no
        #region Propertys
        public string TableName
        {
            get
            {
                return _sTableName;
            }
            set
            {
                _sTableName = value;
            }
        }

        public string KeyValue
        {
            get
            {
                return _sKeyValue;
            }
            set
            {
                _sKeyValue = value;
            }
        }

        public string KeyIdentifier
        {
            get
            {
                return _sKeyIdentifier;
            }
            set
            {
                _sKeyIdentifier = value;
            }
        }

        public string Version
        {
            get
            {
                return _sVersion;
            }
            set
            {
                _sVersion = value;
            }
        }
        #endregion
#endif
        #region Base Class Methods

         public AmcDataConnection GetDataConn()
         {
             return _dbConn;
         }       


        override public void UpdateTable()
        {
            // ja - create the where statement for the update
            GetDataConn().AddUpdateSQLWhere(KeyIdentifier, KeyValue);

            // ja - update the data with the data from SetValue()
            GetDataConn().performUpdate();
        }

        public string GetWhereSQL()
        {
            // ja - create the where statement for "Key" lookup and data population
            string sFormattedKeyValue = AmcDataConnection.QUOTE + KeyValue + AmcDataConnection.QUOTE;
            string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;


            // ja - for 2nd part of where statement 
            // TODO: ja - make this generic
            if (!String.IsNullOrEmpty(Version)){
                sWhere += "and Version = " + AmcDataConnection.QUOTE + Version + AmcDataConnection.QUOTE;
            }           

            return sWhere;
        }

        #endregion

        #region Abstract Functions
        override public void AddColums()
        {

        }
        #endregion

        public string GetPartNumberTable()
        {
            return GetPartNumberTable(KeyValue, Version);            
        }

        public static string GetPartNumberTable(string sPartNumber, string sVersion)
        {
            AmcDataConnection vfpTempConn = new AmcDataConnection(dbType.vfp);
            
            string sFormattedPartNum = AmcDataConnection.QUOTE + sPartNumber + AmcDataConnection.QUOTE;
            string sFormattedVersion = AmcDataConnection.QUOTE + sVersion + AmcDataConnection.QUOTE;
            string sWhere = "Partnumber" + " = " + sFormattedPartNum + "and Version = " + sFormattedVersion;
            string sdbName = "beta.dbf";

            bool bFound = false;
            int i;

            for (i = 0; i < 4; i++)
            {

                if (i == 0)
                    sdbName = "beta.dbf";
                else if (i == 1)
                    sdbName = "current.dbf";
                else if (i == 2)
                    sdbName = "proto.dbf";
                else if (i == 3)
                    sdbName = "history.dbf";

                vfpTempConn.connect(sdbName);

                string sSelectSql = "select partnumber from ";
                sSelectSql += sdbName + " where " + sWhere;
                OleDbCommand cmdSelect = new OleDbCommand(sSelectSql, vfpTempConn.GetConn());

                OleDbDataReader rdr = cmdSelect.ExecuteReader();

                rdr.Read();

                if (rdr.HasRows)
                {
                    bFound = true;

                    break;
                }

            } // ja - end loop

            if (!bFound)
            {
                sdbName = "";
            }

            return sdbName;
        }        
    }
}
