/* Database Type Class
 * 
 * This class must inherit from DBWorks
 * 
 * The Constructor must assign the DatabaseType and Connection String
 * 
 * Other Methods in this class are specific to the Database type we are connecting to
 * 
 */

namespace AMCDatabase
{
    public class EcoTracker : DBWorks
    {
        public string CONNECTION_STRING = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=prodmansql;Password=pr0dman5ux;Initial Catalog=ECO_Tracker;Data Source=HARLEQUIN";
        public int DATABASE_TYPE = (int)dbType.sql;

        public EcoTracker()
        {
            ConnectionString = CONNECTION_STRING; 
            DataBaseType = DATABASE_TYPE;
        }

//         override public string GetWhereSQL()
//         {
//             // ja - create the where statement for "Key" lookup and data population
//             string sFormattedKeyValue = AmcDataConnection.QUOTE + KeyValue + AmcDataConnection.QUOTE;
//             string sWhere = KeyIdentifier + " = " + sFormattedKeyValue;
// 
//             sWhere += AddVersionToWhere();
// 
//             return sWhere;
//         }
// 
//         override public string AddVersionToWhere()
//         {
//             string sVersionWhere = "";
//             
//             return sVersionWhere;
//         }
    }
}
