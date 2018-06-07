/* 
 * 
 * 
 * 
 * The Constructor must assign the DatabaseType and Connection String
 * 
 * Other Methods in this class are specific to the Database type we are connecting to
 * 
 */
using System;
using System.Data.OleDb;

namespace AMCDatabase
{   
    public class PmArchiveTable : PMTables
    {
        const string FINISHEDPRODUCTS = @"\\BOA\Production\Finished_Products\prodHW2";
        const string ARCHIVE = @"\\BOA\Production\Archive";

        override public string GetPMConnString()
        {
            string sPath = GetArchivePath();
             
            // ja - create the connection string
            return "Data Source=" + sPath + "\\" + TableName + ".dbf;" + @"Provider=VFPOLEDB.1;";
        }

        private string GetArchivePath()
        {
            PartsTable pt = new PartsTable(KeyValue, Version);
            string sPath = "";
            string sHworvd = pt.GetDataTable().Rows[0]["hworvd"].ToString().Trim();
            string sProject = pt.GetDataTable().Rows[0]["project"].ToString().Trim();

            string sZeros = "";
            decimal dVersion = Convert.ToDecimal(Version);
            int nVersion = (int)(dVersion * 100);
            if (nVersion < 10)
                sZeros += "0";

            // ja - test for base directory
            if (sHworvd.Trim() == "HIWIRE")
                sPath = FINISHEDPRODUCTS;
            else
                sPath = ARCHIVE;

            // ja - create path
            sPath += "\\" + sProject.Trim();
            sPath += "\\" + KeyValue;
            sPath += "\\" + sZeros + nVersion.ToString();

            return sPath;
        }

    }
}
