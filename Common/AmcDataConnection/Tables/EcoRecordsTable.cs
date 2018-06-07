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
    public class EcoRecordsTable : EcoTracker, ITable
    { 
        const string KEY_IDENTIFIER = "PRN";
        const string TABLE_NAME = "Records";
        
        public EcoRecordsTable(string sKeyValue)
        {
            KeyValue = sKeyValue;

            InitTable();
        }

        public bool InitTable()
        {
            KeyIdentifier = KEY_IDENTIFIER;
            TableName = TABLE_NAME;

            return InitializeDatabase();
        }

        override public string GetWhereSQL()
        {
            // ja - create the where statement for "Key" lookup and data population
            string sFormattedKeyValue = AmcDataConnection.SINGLE_QUOTE + KeyValue + AmcDataConnection.SINGLE_QUOTE;
            string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;

            sWhere += AddVersionToWhere();

            return sWhere;
        }

        override public void AddColumns()
        {
            _ColumnNames.Add("Status");
            _ColumnNames.Add("Text1");
            _ColumnNames.Add("Text4");
            _ColumnNames.Add("Text5");
            _ColumnNames.Add("Text6");
            _ColumnNames.Add("Text7");
            _ColumnNames.Add("Text8");
            _ColumnNames.Add("Text9");
            _ColumnNames.Add("Text12");
            _ColumnNames.Add("Text17");
            _ColumnNames.Add("Text18");
            _ColumnNames.Add("Text19");
            _ColumnNames.Add("Pulldown0");
        }

        //         static void caller(String myclass, String mymethod)
        //         {
        //             // Get a type from the string 
        //             Type type = Type.GetType(myclass);
        //             // Create an instance of that type
        //             Object obj = Activator.CreateInstance(type);
        //             // Retrieve the method you are looking for
        //             MethodInfo methodInfo = type.GetMethod(mymethod);
        //             // Invoke the method on the instance we created above
        //             methodInfo.Invoke(obj, null);
        //         }
    }
}
