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

namespace AMCDatabase
{
    public class Name : PMTables, ITable
    {
        public Name(string sKeyValue, bool bTest = false)
        {
            KeyValue = sKeyValue;
            TestData = bTest;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = "Username";
            TableName = "Name";

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }
#if no
        public override void AddColums()
        {
            // TODO: ja - add all columns

            _ColumnNames.Add("Fullname");
            _ColumnNames.Add("Username");            
        }
#endif
    }
}
 