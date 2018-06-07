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
    public class PMLabels : DBWorks
    {
        public string CONNECTION_STRING = "User Id=autest;password=autest1234;server=amc-sql01;database=AMC_MfgData;connection timeout=30";
        //public string CONNECTION_STRING = "Provider=SQLOLEDB;Data Source=BUSH*MASTER;Initial Catalog=ProductionManualLabels;Integrated security=SSPI;";
        public int DATABASE_TYPE = (int)dbType.sql;

        public PMLabels()
        {
            ConnectionString = CONNECTION_STRING;
            DataBaseType = DATABASE_TYPE;
        }
    }
}
