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
    public class Workcode : PMTables, ITable
    {
        public string MODELLCODE = "Modellcode";  
        public string AMPVERSION = "Ampver";
        public string TYPEWO = "typewo";

        public Workcode(string sKeyValue, bool bTest = false)
        {
            KeyValue = sKeyValue;
            TestData = bTest;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = "Workcode";
            TableName = "Workcode";

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }
#if no        
        public override void AddColums()
        {
            // TODO: ja - add all columns
            
            _ColumnNames.Add(KeyIdentifier);
            _ColumnNames.Add(MODELLCODE);
            _ColumnNames.Add("Quantity");
            _ColumnNames.Add(AMPVERSION);
            _ColumnNames.Add("Ems_wover");
        }       
#endif
    }
}

