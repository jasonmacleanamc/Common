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
    public class Amplifs : PMTables, ITable
    {
        public string BASEPLATE = "Baseplate";
        public string MODELLCODE = "Modellcode";
        
        // ja - test
        public string PREGTEST = "pregtest";
        public string FREGTEST = "fregtest";
        public string REGTEST = "regtest";
        public string PICTTEST = "picttest";
        public string TESTS = "tests";
        public string TEST_STATION = "test_station";

        // ja - burn
        public string PREGBURN = "pregburn";
        public string FREGBURN = "fregburn";
        public string BURNS = "burns";
        public string BURN_STATION = "burn_station";
        public string IN_BOX = "in_box";
        
        public string YLDTYPE = "yldtype";
        public string AMPMEMO = "ampmemo";
        public string MODIFYTIME = "modifytime";

        public Amplifs(string sKeyValue, bool bTest = false)
        {
            KeyValue = sKeyValue;
            TestData = bTest;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = "Baseplate";
            TableName = "Amplifs";

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }
        
#if no
        public override void AddColums() 
        {
            // TODO: ja - add all columns
            _ColumnNames.Add("Pregburn");
        }
#endif
    }
}
