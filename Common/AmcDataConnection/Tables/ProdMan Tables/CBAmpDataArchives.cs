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
    public class CBAmpDataArchives : PmArchiveTable, ITable, IKeyPart
    {
        public CBAmpDataArchives(string sKeyValue, string sVersion, bool bTest = false)
        {
            Version = sVersion;
            KeyValue = sKeyValue;
            TestData = bTest;
            SecondKeyValue = sVersion;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = "Ep_part";
            TableName = "CB_EEPROM";
            SecondKeyIdentifier = "Ep_version";

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();
        }

        override public string AddVersionToWhere()
        {
            string sVersionWhere = "";

            // ja - for 2nd part of where statement 
            if (!string.IsNullOrEmpty(Version))
            {
                sVersionWhere = "and  " + SecondKeyIdentifier + " = " + AmcDataConnection.QUOTE + SecondKeyValue + AmcDataConnection.QUOTE;
            }

            return sVersionWhere;
        }

        public override void AddColumns() 
        {
            // ja - add all columns
            _ColumnNames.Add(KeyIdentifier);
            _ColumnNames.Add(SecondKeyIdentifier);
            _ColumnNames.Add("EP_Name");
        }
    }
}
