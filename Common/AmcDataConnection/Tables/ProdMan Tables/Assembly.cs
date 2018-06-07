/* ITable Class
 * 
 * This class must inherit from the Database Type (derived from DBWorks) class and the ITable Interface
 * 
 * The Constructor must assign the KeyValue Property and call InitTable() 
 * 
 * InitTable() must assign the KeyIdentifier and TableName Property and call InitializeDatabase()
 * 
 * AddColums() must populate the global _ColumnNames field
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    public class Assembly : PMTables, ITable
    {
        //string x1 = "assycurr";

        public Assembly(string sKeyValue, string sVersion, bool bTest = false)
        {
            KeyValue = sKeyValue;
            Version = sVersion;
            TestData = bTest;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = "Partnumber";

            string sTableName = PartsTable.GetPartNumberTable(KeyValue, Version);
            TableName = sTableName;

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }
#if no
        public override void AddColums()
        {
            // TODO: ja - add all columns 

            _ColumnNames.Add(KeyIdentifier);            

        }
#endif
    }

}
