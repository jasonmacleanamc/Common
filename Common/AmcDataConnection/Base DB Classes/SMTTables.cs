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
    public class SMTTables : PMTables
    {        
        public SMTTables()
        {
            DataBaseType = (int)dbType.vfp;
        }

        override public string GetPMConnString()
        {        
            // ja - get the temp dir and add prodloc..
            string tempPath = System.IO.Path.GetTempPath();

            // ja - create the connection string
            return @"Data Source=" + tempPath + @"prodloc\" + TableName + ".dbf;" + @"Provider=VFPOLEDB.1;";
        }
    }
}
